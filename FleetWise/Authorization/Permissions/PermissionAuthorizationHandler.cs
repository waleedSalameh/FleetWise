using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FleetWise.API.Authorization.Permissions;

public class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        var tokenSecurityStamp = context.User.FindFirst("security_stamp")?.Value;
        if (string.IsNullOrWhiteSpace(tokenSecurityStamp))
        {
            return;
        }

        var allowed = await permissionService.HasPermissionAsync(
            userId,
            requirement.PermissionCode,
            tokenSecurityStamp
        );

        if (allowed)
        {
            context.Succeed(requirement);
        }
    }
}
