using Microsoft.AspNetCore.Identity;

namespace FleetWise.DAL.Entities.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
}
