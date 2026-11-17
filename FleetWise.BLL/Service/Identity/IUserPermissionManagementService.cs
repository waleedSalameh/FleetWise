using FleetWise.BLL.DTOs.Identity;

namespace FleetWise.BLL.Service.Identity;

public interface IUserPermissionManagementService
{
    Task<List<UserPermissionStatusDto>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    Task SetUserOverrideAsync(
        string actorUserId,
        string userId,
        SetUserPermissionOverrideDto dto,
        CancellationToken cancellationToken = default
    );

    Task<bool> RemoveUserOverrideAsync(
        string actorUserId,
        string userId,
        string permissionCode,
        CancellationToken cancellationToken = default
    );
}
