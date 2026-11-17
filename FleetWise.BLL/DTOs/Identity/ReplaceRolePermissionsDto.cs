using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class ReplaceRolePermissionsDto
{
    [Required]
    public List<string> PermissionCodes { get; set; } = new();
}
