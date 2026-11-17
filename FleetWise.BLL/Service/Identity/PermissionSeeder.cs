using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetWise.BLL.Service.Identity;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default
    )
    {
        await using var scope = services.CreateAsyncScope();

        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<FleetWiseDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var catalogCodes = PermissionCatalog.All.Select(permission => permission.Code).ToArray();

        var existingCodes = await dbContext
            .Permissions.AsNoTracking()
            .Select(permission => permission.Code)
            .ToHashSetAsync(cancellationToken);

        var missingPermissions = PermissionCatalog
            .All.Where(permission => !existingCodes.Contains(permission.Code))
            .Select(permission => new Permission
            {
                Code = permission.Code,
                Name = permission.Name,
                Module = permission.Module,
            })
            .ToList();

        if (missingPermissions.Count > 0)
        {
            await dbContext.Permissions.AddRangeAsync(missingPermissions, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var adminRole = await roleManager.FindByNameAsync("Admin");

        if (adminRole is null)
        {
            adminRole = new IdentityRole("Admin");
            var result = await roleManager.CreateAsync(adminRole);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));

                throw new InvalidOperationException($"Could not create the Admin role: {errors}");
            }
        }

        var catalogPermissions = await dbContext
            .Permissions.Where(permission => catalogCodes.Contains(permission.Code))
            .ToListAsync(cancellationToken);

        var existingPermissionIds = await dbContext
            .RolePermissions.Where(rolePermission => rolePermission.RoleId == adminRole.Id)
            .Select(rolePermission => rolePermission.PermissionId)
            .ToHashSetAsync(cancellationToken);

        var missingRolePermissions = catalogPermissions
            .Where(permission => !existingPermissionIds.Contains(permission.Id))
            .Select(permission => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id,
            })
            .ToList();

        if (missingRolePermissions.Count > 0)
        {
            await dbContext.RolePermissions.AddRangeAsync(
                missingRolePermissions,
                cancellationToken
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
