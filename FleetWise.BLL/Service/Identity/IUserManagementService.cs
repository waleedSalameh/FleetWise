using FleetWise.BLL.DTOs.Identity;

namespace FleetWise.BLL.Service.Identity;

public interface IUserManagementService
{
    Task<List<UserSummaryDto>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    );

    Task<UserSummaryDto> CreateAsync(
        string actorUserId,
        CreateUserDto dto,
        CancellationToken cancellationToken = default
    );

    Task SetActiveAsync(
        string actorUserId,
        string userId,
        bool isActive,
        CancellationToken cancellationToken = default
    );

    Task ResetPasswordAsync(
        string actorUserId,
        string userId,
        ResetUserPasswordDto dto,
        CancellationToken cancellationToken = default
    );
}
