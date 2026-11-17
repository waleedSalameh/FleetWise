namespace FleetWise.BLL.DTOs.Identity;

public sealed class RoleSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }
}
