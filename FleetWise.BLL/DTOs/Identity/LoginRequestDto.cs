using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public sealed class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
