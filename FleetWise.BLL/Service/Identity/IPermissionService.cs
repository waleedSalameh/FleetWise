namespace FleetWise.BLL.Service.Identity;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        string userId,
        string permissionCode,
        string tokenSecurityStamp,
        CancellationToken cancellationToken = default
    );
}
