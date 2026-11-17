using Microsoft.AspNetCore.Identity;

namespace FleetWise.DAL.Entities.Identity;

public class RolePermission
{
    public string RoleId { get; set; } = string.Empty;

    public IdentityRole Role { get; set; } = null!;

    public int PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}
