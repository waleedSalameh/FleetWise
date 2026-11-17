using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Identity;

public class SetUserPermissionOverrideDto
{
    [Required]
    [MaxLength(100)]
    public string PermissionCode { get; set; } = string.Empty;

    public bool IsAllowed { get; set; }
}
