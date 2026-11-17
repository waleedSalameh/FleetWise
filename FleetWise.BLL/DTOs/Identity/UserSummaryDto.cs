namespace FleetWise.BLL.DTOs.Identity;

public class UserSummaryDto
{
    public string Id { get; set; } = string.Empty;

    public string? Email { get; set; }

    public bool IsActive { get; set; }
}
