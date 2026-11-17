using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class ReplaceUserRolesDto
{
    [Required]
    public List<string> RoleIds { get; set; } = new();
}
