using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FleetWise.API.Authorization.Permissions;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private const string PermissionPolicyPrefix = "Permission:";

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var existingPolicy = await base.GetPolicyAsync(policyName);

        if (existingPolicy is not null)
        {
            return existingPolicy;
        }

        if (!policyName.StartsWith(PermissionPolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var permissionCode = policyName[PermissionPolicyPrefix.Length..];

        if (string.IsNullOrWhiteSpace(permissionCode))
        {
            return null;
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissionCode))
            .Build();
    }
}
