using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public class PermissionService(FleetWiseDbContext dbContext) : IPermissionService
{
    private static readonly HashSet<string> AdminOnlyPermissions =
        ["Roles.Manage", "Permissions.Manage"];

    public async Task<bool> HasPermissionAsync(
        string userId,
        string permissionCode,
        string tokenSecurityStamp,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(permissionCode))
        {
            return false;
        }

        var userState = await dbContext
            .Users.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new { user.IsActive, user.SecurityStamp })
            .SingleOrDefaultAsync(cancellationToken);

        if (
            userState is null
            || !userState.IsActive
            || string.IsNullOrWhiteSpace(tokenSecurityStamp)
            || !string.Equals(userState.SecurityStamp, tokenSecurityStamp, StringComparison.Ordinal)
        )
        {
            return false;
        }

        if (permissionCode is "Roles.Manage" or "Permissions.Manage")
        {
            return await dbContext.UserRoles
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
        }

        var userOverride = await dbContext
            .UserPermissionOverrides.AsNoTracking()
            .Where(permissionOverride =>
                permissionOverride.UserId == userId
                && permissionOverride.Permission.Code == permissionCode
            )
            .Select(permissionOverride => (bool?)permissionOverride.IsAllowed)
            .SingleOrDefaultAsync(cancellationToken);

        if (userOverride.HasValue)
        {
            return userOverride.Value;
        }

        var userRoleIds = dbContext
            .UserRoles.Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.RoleId);

        return await dbContext
            .RolePermissions.AsNoTracking()
            .AnyAsync(
                rolePermission =>
                    userRoleIds.Contains(rolePermission.RoleId)
                    && rolePermission.Permission.Code == permissionCode,
                cancellationToken
            );
    }

    public async Task<FleetWise.BLL.DTOs.Identity.CurrentUserPermissionsDto?> GetCurrentUserPermissionsAsync(
        string userId,
        string tokenSecurityStamp,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(tokenSecurityStamp))
        {
            return null;
        }

        var userState = await dbContext
            .Users.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new { user.IsActive, user.SecurityStamp })
            .SingleOrDefaultAsync(cancellationToken);

        if (
            userState is null
            || !userState.IsActive
            || !string.Equals(userState.SecurityStamp, tokenSecurityStamp, StringComparison.Ordinal)
        )
        {
            return null;
        }

        var userRoleIds = await dbContext
            .UserRoles.AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.RoleId)
            .ToListAsync(cancellationToken);

        var rolePermissionCodes = await dbContext
            .RolePermissions.AsNoTracking()
            .Where(rolePermission => userRoleIds.Contains(rolePermission.RoleId))
            .Select(rolePermission => rolePermission.Permission.Code)
            .ToListAsync(cancellationToken);
        var rolePermissionCodeSet = rolePermissionCodes.ToHashSet(StringComparer.Ordinal);

        var userOverrides = await dbContext
            .UserPermissionOverrides.AsNoTracking()
            .Where(permissionOverride => permissionOverride.UserId == userId)
            .Select(permissionOverride => new
            {
                permissionOverride.Permission.Code,
                permissionOverride.IsAllowed,
            })
            .ToDictionaryAsync(
                permissionOverride => permissionOverride.Code,
                permissionOverride => permissionOverride.IsAllowed,
                StringComparer.Ordinal,
                cancellationToken
            );

        var isAdmin = await dbContext
            .UserRoles.Join(
                dbContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.UserId, role.Name }
            )
            .AnyAsync(
                userRole => userRole.UserId == userId && userRole.Name == "Admin",
                cancellationToken
            );

        var permissionCodes = await dbContext
            .Permissions.AsNoTracking()
            .OrderBy(permission => permission.Code)
            .Select(permission => permission.Code)
            .ToListAsync(cancellationToken);

        var effectivePermissions = permissionCodes
            .Where(permissionCode =>
            {
                if (AdminOnlyPermissions.Contains(permissionCode))
                {
                    return isAdmin;
                }

                return userOverrides.TryGetValue(permissionCode, out var userOverride)
                    ? userOverride
                    : rolePermissionCodeSet.Contains(permissionCode);
            })
            .ToList();

        return new FleetWise.BLL.DTOs.Identity.CurrentUserPermissionsDto
        {
            Permissions = effectivePermissions,
        };
    }
}
