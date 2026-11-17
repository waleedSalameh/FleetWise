using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class CreateUserDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(12)]
    public string TemporaryPassword { get; set; } = string.Empty;
}
