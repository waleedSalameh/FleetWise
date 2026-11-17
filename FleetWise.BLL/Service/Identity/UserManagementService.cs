using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Concurrency;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FleetWise.BLL.Service.Identity;

public sealed class UserManagementService(
    FleetWiseDbContext dbContext,
    UserManager<ApplicationUser> userManager
) : IUserManagementService
{
    public async Task<List<UserSummaryDto>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be at least 1."
            );
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 100."
            );
        }

        var skip = checked((pageNumber - 1) * pageSize);

        return await dbContext
            .Users.AsNoTracking()
            .OrderBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(user => new UserSummaryDto
            {
                Id = user.Id,
                Email = user.Email,
                IsActive = user.IsActive,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSummaryDto> CreateAsync(
        string actorUserId,
        CreateUserDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var email = dto.Email.Trim();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            EmailConfirmed = false,
            IsActive = true,
        };

        EnsureIdentityResult(await userManager.CreateAsync(user, dto.TemporaryPassword));

        AddAudit(actorUserId, "UserCreated", user.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new UserSummaryDto { Id = user.Id, Email = user.Email, IsActive = user.IsActive };
    }

    public async Task SetActiveAsync(
        string actorUserId,
        string userId,
        bool isActive,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User was not found.");

        if (user.IsActive == isActive)
        {
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken
        );

        var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
        if (!isActive && user.IsActive && isAdmin)
        {
            await DatabaseApplicationLock.AcquireAsync(
                dbContext,
                "FleetWise:Admin:LastActiveAdmin",
                cancellationToken
            );

            if (await CountActiveAdminsAsync(cancellationToken) <= 1)
            {
                throw new InvalidOperationException("The last active administrator cannot be disabled.");
            }
        }

        user.IsActive = isActive;
        EnsureIdentityResult(await userManager.UpdateAsync(user));
        EnsureIdentityResult(await userManager.UpdateSecurityStampAsync(user));

        if (!isActive)
        {
            var revokedAtUtc = DateTime.UtcNow;
            await dbContext.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAtUtc == null)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(token => token.RevokedAtUtc, revokedAtUtc),
                    cancellationToken
                );
        }

        AddAudit(actorUserId, isActive ? "UserActivated" : "UserDeactivated", user.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ResetPasswordAsync(
        string actorUserId,
        string userId,
        ResetUserPasswordDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User was not found.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        EnsureIdentityResult(await userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword));
        EnsureIdentityResult(await userManager.UpdateSecurityStampAsync(user));

        var revokedAtUtc = DateTime.UtcNow;
        await dbContext.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAtUtc == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(token => token.RevokedAtUtc, revokedAtUtc),
                cancellationToken
            );

        AddAudit(actorUserId, "UserPasswordReset", user.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(user => user.IsActive)
            .Join(dbContext.UserRoles, user => user.Id, userRole => userRole.UserId, (_, userRole) => userRole.RoleId)
            .Join(dbContext.Roles, roleId => roleId, role => role.Id, (_, role) => role.Name)
            .CountAsync(roleName => roleName == "Admin", cancellationToken);
    }

    private void AddAudit(string actorUserId, string action, string userId)
    {
        dbContext.AdminAuditLogs.Add(new AdminAuditLog
        {
            ActorUserId = actorUserId,
            Action = action,
            TargetType = "User",
            TargetId = userId,
            OccurredAtUtc = DateTime.UtcNow,
        });
    }

    private static void EnsureIdentityResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new ArgumentException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }
}
