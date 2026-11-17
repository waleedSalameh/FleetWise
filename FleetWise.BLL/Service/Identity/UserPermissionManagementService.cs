using FleetWise.BLL.DTOs.Identity;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public sealed class UserPermissionManagementService(FleetWiseDbContext dbContext)
    : IUserPermissionManagementService
{
    public async Task<List<UserPermissionStatusDto>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var userExists = await dbContext.Users.AnyAsync(
            user => user.Id == userId && user.IsActive,
            cancellationToken
        );

        if (!userExists)
        {
            throw new KeyNotFoundException("Active user was not found.");
        }

        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Module)
            .ThenBy(permission => permission.Code)
            .ToListAsync(cancellationToken);

        var rolePermissionIds = await dbContext.RolePermissions
            .AsNoTracking()
            .Where(rolePermission =>
                dbContext.UserRoles.Any(userRole =>
                    userRole.UserId == userId
                    && userRole.RoleId == rolePermission.RoleId
                )
            )
            .Select(rolePermission => rolePermission.PermissionId)
            .ToListAsync(cancellationToken);

        var rolePermissionIdSet = rolePermissionIds.ToHashSet();
        var isAdmin = await dbContext.UserRoles
            .Join(
                dbContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.UserId, role.Name }
            )
            .AnyAsync(
                userRole => userRole.UserId == userId && userRole.Name == "Admin",
                cancellationToken
            );

        var userOverrides = await dbContext.UserPermissionOverrides
            .AsNoTracking()
            .Where(permissionOverride => permissionOverride.UserId == userId)
            .ToDictionaryAsync(
                permissionOverride => permissionOverride.PermissionId,
                permissionOverride => permissionOverride.IsAllowed,
                cancellationToken
            );

        return permissions
            .Select(permission =>
            {
                bool? userOverride = userOverrides.TryGetValue(
                    permission.Id,
                    out var isAllowed
                )
                    ? isAllowed
                    : null;

                return new UserPermissionStatusDto
                {
                    PermissionCode = permission.Code,
                    Name = permission.Name,
                    Module = permission.Module,
                    UserOverride = userOverride,
                    IsEffectivelyAllowed =
                        (permission.Code is "Roles.Manage" or "Permissions.Manage")
                            ? isAdmin
                            : userOverride ?? rolePermissionIdSet.Contains(permission.Id),
                };
            })
            .ToList();
    }

    public async Task SetUserOverrideAsync(
        string actorUserId,
        string userId,
        SetUserPermissionOverrideDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var userExists = await dbContext.Users.AnyAsync(
            user => user.Id == userId && user.IsActive,
            cancellationToken
        );

        if (!userExists)
        {
            throw new KeyNotFoundException("Active user was not found.");
        }

        var permissionCode = dto.PermissionCode.Trim();

        // هاي الصلاحيات الإدارية تظل مرتبطة بدور Admin،
        // وما بنعطيها كـ override لمستخدم.
        if (permissionCode is "Permissions.Manage" or "Roles.Manage")
        {
            throw new InvalidOperationException(
                "Administrative permissions cannot be assigned as user overrides."
            );
        }

        var permission = await dbContext.Permissions.SingleOrDefaultAsync(
            item => item.Code == permissionCode,
            cancellationToken
        );

        if (permission is null)
        {
            throw new ArgumentException("Permission code was not found.");
        }

        var existingOverride = await dbContext.UserPermissionOverrides.SingleOrDefaultAsync(
            item => item.UserId == userId && item.PermissionId == permission.Id,
            cancellationToken
        );

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (existingOverride is null)
        {
            dbContext.UserPermissionOverrides.Add(
                new UserPermissionOverride
                {
                    UserId = userId,
                    PermissionId = permission.Id,
                    IsAllowed = dto.IsAllowed,
                }
            );
        }
        else
        {
            existingOverride.IsAllowed = dto.IsAllowed;
        }

        dbContext.AdminAuditLogs.Add(new AdminAuditLog
        {
            ActorUserId = actorUserId,
            Action = dto.IsAllowed ? "UserPermissionAllowed" : "UserPermissionDenied",
            TargetType = "User",
            TargetId = userId,
            ChangeSummary = permissionCode,
            OccurredAtUtc = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<bool> RemoveUserOverrideAsync(
        string actorUserId,
        string userId,
        string permissionCode,
        CancellationToken cancellationToken = default
    )
    {
        var userExists = await dbContext.Users.AnyAsync(
            user => user.Id == userId && user.IsActive,
            cancellationToken
        );

        if (!userExists)
        {
            throw new KeyNotFoundException("Active user was not found.");
        }

        var normalizedCode = permissionCode.Trim();

        var permission = await dbContext.Permissions.SingleOrDefaultAsync(
            item => item.Code == normalizedCode,
            cancellationToken
        );

        if (permission is null)
        {
            throw new ArgumentException("Permission code was not found.");
        }

        var existingOverride = await dbContext.UserPermissionOverrides
            .SingleOrDefaultAsync(
                item => item.UserId == userId && item.PermissionId == permission.Id,
                cancellationToken
            );

        if (existingOverride is null)
        {
            return false;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.UserPermissionOverrides.Remove(existingOverride);
        dbContext.AdminAuditLogs.Add(new AdminAuditLog
        {
            ActorUserId = actorUserId,
            Action = "UserPermissionOverrideRemoved",
            TargetType = "User",
            TargetId = userId,
            ChangeSummary = normalizedCode,
            OccurredAtUtc = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }
}
