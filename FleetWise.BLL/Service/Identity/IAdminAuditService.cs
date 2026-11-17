using FleetWise.BLL.DTOs.Identity;

namespace FleetWise.BLL.Service.Identity;

public interface IAdminAuditService
{
    Task<List<AdminAuditEntryDto>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default
    );
}
