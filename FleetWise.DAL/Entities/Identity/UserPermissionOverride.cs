namespace FleetWise.DAL.Entities.Identity;

public class UserPermissionOverride
{
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public int PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public bool IsAllowed { get; set; }
}
