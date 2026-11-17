namespace FleetWise.BLL.DTOs.Identity;

public sealed class RolePermissionStatusDto
{
    public string PermissionCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}
