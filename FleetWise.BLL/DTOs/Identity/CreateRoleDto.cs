using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class CreateRoleDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}
