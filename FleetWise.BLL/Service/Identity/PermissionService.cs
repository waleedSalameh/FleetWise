using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public class PermissionService(FleetWiseDbContext dbContext) : IPermissionService
{
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
}
