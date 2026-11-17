namespace FleetWise.BLL.Service.Identity;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        string userId,
        string permissionCode,
        string tokenSecurityStamp,
        CancellationToken cancellationToken = default
    );

    Task<FleetWise.BLL.DTOs.Identity.CurrentUserPermissionsDto?> GetCurrentUserPermissionsAsync(
        string userId,
        string tokenSecurityStamp,
        CancellationToken cancellationToken = default
    );
}
