using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class ResetUserPasswordDto
{
    [Required]
    [MinLength(12)]
    public string NewPassword { get; set; } = string.Empty;
}
