using System.Data;
using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Concurrency;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public sealed class RoleManagementService(
    FleetWiseDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager
) : IRoleManagementService
{
    public async Task<List<RoleSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .Select(role => new RoleSummaryDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UserCount = dbContext.UserRoles.Count(userRole => userRole.RoleId == role.Id),
                PermissionCount = dbContext.RolePermissions.Count(rolePermission => rolePermission.RoleId == role.Id),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleSummaryDto> CreateAsync(
        string actorUserId,
        CreateRoleDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var name = NormalizeRoleName(dto.Name);
        if (await roleManager.RoleExistsAsync(name))
        {
            throw new InvalidOperationException("A role with this name already exists.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var role = new IdentityRole(name);
        EnsureIdentityResult(await roleManager.CreateAsync(role));
        AddAudit(actorUserId, "RoleCreated", "Role", role.Id, $"Name={name}");
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new RoleSummaryDto { Id = role.Id, Name = role.Name ?? name };
    }

    public async Task<RoleSummaryDto> UpdateAsync(
        string actorUserId,
        string roleId,
        UpdateRoleDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var role = await FindRoleAsync(roleId);
        EnsureMutableRole(role);
        var name = NormalizeRoleName(dto.Name);

        if (await roleManager.RoleExistsAsync(name) && !string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("A role with this name already exists.");
        }

        var oldName = role.Name ?? string.Empty;
        role.Name = name;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        EnsureIdentityResult(await roleManager.UpdateAsync(role));
        AddAudit(actorUserId, "RoleRenamed", "Role", role.Id, $"{oldName} -> {name}");
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new RoleSummaryDto { Id = role.Id, Name = role.Name ?? name };
    }

    public async Task DeleteAsync(
        string actorUserId,
        string roleId,
        CancellationToken cancellationToken = default
    )
    {
        var role = await FindRoleAsync(roleId);
        EnsureMutableRole(role);

        if (await dbContext.UserRoles.AnyAsync(userRole => userRole.RoleId == role.Id, cancellationToken))
        {
            throw new InvalidOperationException("Remove this role from all users before deleting it.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var name = role.Name ?? string.Empty;
        EnsureIdentityResult(await roleManager.DeleteAsync(role));
        AddAudit(actorUserId, "RoleDeleted", "Role", role.Id, $"Name={name}");
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<List<RolePermissionStatusDto>> GetRolePermissionsAsync(
        string roleId,
        CancellationToken cancellationToken = default
    )
    {
        var role = await FindRoleAsync(roleId);
        var assignedIds = await dbContext.RolePermissions
            .AsNoTracking()
            .Where(rolePermission => rolePermission.RoleId == role.Id)
            .Select(rolePermission => rolePermission.PermissionId)
            .ToHashSetAsync(cancellationToken);

        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Module)
            .ThenBy(permission => permission.Code)
            .ToListAsync(cancellationToken);

        return permissions.Select(permission => new RolePermissionStatusDto
            {
                PermissionCode = permission.Code,
                Name = permission.Name,
                Module = permission.Module,
                IsAssigned = assignedIds.Contains(permission.Id),
            })
            .ToList();
    }

    public async Task ReplaceRolePermissionsAsync(
        string actorUserId,
        string roleId,
        ReplaceRolePermissionsDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var role = await FindRoleAsync(roleId);
        EnsureMutableRole(role);
        var codes = NormalizeCodes(dto.PermissionCodes);

        if (codes.Any(code => code is "Roles.Manage" or "Permissions.Manage"))
        {
            throw new InvalidOperationException("Administrative permissions can only belong to the Admin role.");
        }

        var permissions = await dbContext.Permissions
            .Where(permission => codes.Contains(permission.Code))
            .ToListAsync(cancellationToken);
        if (permissions.Count != codes.Count)
        {
            throw new ArgumentException("One or more permission codes were not found.");
        }

        var selectedIds = permissions.Select(permission => permission.Id).ToHashSet();
        var existing = await dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == role.Id)
            .ToListAsync(cancellationToken);
        var currentIds = existing.Select(item => item.PermissionId).ToHashSet();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var removed = existing.Where(item => !selectedIds.Contains(item.PermissionId)).ToList();
        dbContext.RolePermissions.RemoveRange(removed);
        var added = permissions
            .Where(permission => !currentIds.Contains(permission.Id))
            .Select(permission => new RolePermission { RoleId = role.Id, PermissionId = permission.Id })
            .ToList();
        await dbContext.RolePermissions.AddRangeAsync(added, cancellationToken);

        foreach (var item in removed)
        {
            var code = await dbContext.Permissions.Where(permission => permission.Id == item.PermissionId)
                .Select(permission => permission.Code).SingleAsync(cancellationToken);
            AddAudit(actorUserId, "RolePermissionRemoved", "Role", role.Id, code);
        }
        foreach (var item in added)
        {
            var code = permissions.Single(permission => permission.Id == item.PermissionId).Code;
            AddAudit(actorUserId, "RolePermissionGranted", "Role", role.Id, code);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<List<RoleSummaryDto>> GetUserRolesAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            throw new KeyNotFoundException("User was not found.");
        }

        return await roleManager.Roles
            .AsNoTracking()
            .Where(role => dbContext.UserRoles.Any(userRole => userRole.UserId == userId && userRole.RoleId == role.Id))
            .OrderBy(role => role.Name)
            .Select(role => new RoleSummaryDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UserCount = dbContext.UserRoles.Count(userRole => userRole.RoleId == role.Id),
                PermissionCount = dbContext.RolePermissions.Count(rolePermission => rolePermission.RoleId == role.Id),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceUserRolesAsync(
        string actorUserId,
        string userId,
        ReplaceUserRolesDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User was not found.");
        var requestedIds = (dto.RoleIds ?? new List<string>())
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);
        var requestedRoles = await roleManager.Roles
            .Where(role => requestedIds.Contains(role.Id))
            .ToListAsync(cancellationToken);
        if (requestedRoles.Count != requestedIds.Count)
        {
            throw new ArgumentException("One or more role IDs were not found.");
        }

        var requestedNames = requestedRoles.Select(role => role.Name!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var currentNames = (await userManager.GetRolesAsync(user)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var wasAdmin = currentNames.Contains("Admin");
        var willBeAdmin = requestedNames.Contains("Admin");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        await DatabaseApplicationLock.AcquireAsync(dbContext, "FleetWise:Admin:LastActiveAdmin", cancellationToken);

        if (user.IsActive && wasAdmin && !willBeAdmin && await CountActiveAdminsAsync(cancellationToken) <= 1)
        {
            throw new InvalidOperationException("The last active administrator cannot lose the Admin role.");
        }

        var toRemove = currentNames.Except(requestedNames, StringComparer.OrdinalIgnoreCase).ToArray();
        var toAdd = requestedNames.Except(currentNames, StringComparer.OrdinalIgnoreCase).ToArray();

        EnsureIdentityResult(await userManager.RemoveFromRolesAsync(user, toRemove));
        EnsureIdentityResult(await userManager.AddToRolesAsync(user, toAdd));

        var summary = $"Roles: {string.Join(",", toRemove.Select(name => "-" + name).Concat(toAdd.Select(name => "+" + name)))}";
        AddAudit(actorUserId, "UserRolesReplaced", "User", user.Id, summary);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<IdentityRole> FindRoleAsync(string roleId)
    {
        return await roleManager.FindByIdAsync(roleId)
            ?? throw new KeyNotFoundException("Role was not found.");
    }

    private static void EnsureMutableRole(IdentityRole role)
    {
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The Admin role is protected from modification.");
        }
    }

    private static string NormalizeRoleName(string name)
    {
        var normalized = name.Trim();
        if (normalized.Length < 2 || normalized.Length > 100)
        {
            throw new ArgumentException("Role name must be between 2 and 100 characters.");
        }
        return normalized;
    }

    private static List<string> NormalizeCodes(List<string>? codes)
    {
        if (codes is null)
        {
            throw new ArgumentException("Permission codes are required.");
        }

        if (codes.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Permission codes cannot be empty.");
        }

        var normalized = codes.Select(code => code.Trim()).ToList();
        if (normalized.Distinct(StringComparer.Ordinal).Count() != normalized.Count)
        {
            throw new ArgumentException("Duplicate permission codes are not allowed.");
        }
        return normalized;
    }

    private async Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(user => user.IsActive)
            .Join(dbContext.UserRoles, user => user.Id, userRole => userRole.UserId, (_, userRole) => userRole.RoleId)
            .Join(dbContext.Roles, roleId => roleId, role => role.Id, (roleId, role) => role.Name)
            .CountAsync(roleName => roleName == "Admin", cancellationToken);
    }

    private void AddAudit(string actorUserId, string action, string targetType, string targetId, string? summary)
    {
        dbContext.AdminAuditLogs.Add(new AdminAuditLog
        {
            ActorUserId = actorUserId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            ChangeSummary = summary,
            OccurredAtUtc = DateTime.UtcNow,
        });
    }

    private static void EnsureIdentityResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }
}
