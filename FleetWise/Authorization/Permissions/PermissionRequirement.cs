using Microsoft.AspNetCore.Authorization;

namespace FleetWise.API.Authorization.Permissions;

public class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
