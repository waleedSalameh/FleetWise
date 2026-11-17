namespace FleetWise.BLL.DTOs.Identity;

public sealed class UserPermissionStatusDto
{
    public string PermissionCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    // null means the permission is inherited from a role.
    public bool? UserOverride { get; set; }

    public bool IsEffectivelyAllowed { get; set; }
}
