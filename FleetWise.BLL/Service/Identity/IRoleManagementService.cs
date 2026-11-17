using FleetWise.BLL.DTOs.Identity;

namespace FleetWise.BLL.Service.Identity;

public interface IRoleManagementService
{
    Task<List<RoleSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleSummaryDto> CreateAsync(string actorUserId, CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<RoleSummaryDto> UpdateAsync(string actorUserId, string roleId, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string actorUserId, string roleId, CancellationToken cancellationToken = default);
    Task<List<RolePermissionStatusDto>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken = default);
    Task ReplaceRolePermissionsAsync(string actorUserId, string roleId, ReplaceRolePermissionsDto dto, CancellationToken cancellationToken = default);
    Task<List<RoleSummaryDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task ReplaceUserRolesAsync(string actorUserId, string userId, ReplaceUserRolesDto dto, CancellationToken cancellationToken = default);
}
