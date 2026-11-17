using FleetWise.BLL.DTOs.Identity;
using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public sealed class AdminAuditService(FleetWiseDbContext dbContext) : IAdminAuditService
{
    public async Task<List<AdminAuditEntryDto>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default
    )
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1 || pageSize > 100) throw new ArgumentOutOfRangeException(nameof(pageSize));
        var skip = checked((pageNumber - 1) * pageSize);

        return await dbContext.AdminAuditLogs
            .AsNoTracking()
            .OrderByDescending(entry => entry.OccurredAtUtc)
            .ThenByDescending(entry => entry.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(entry => new AdminAuditEntryDto
            {
                Id = entry.Id,
                ActorUserId = entry.ActorUserId,
                Action = entry.Action,
                TargetType = entry.TargetType,
                TargetId = entry.TargetId,
                ChangeSummary = entry.ChangeSummary,
                OccurredAtUtc = entry.OccurredAtUtc,
            })
            .ToListAsync(cancellationToken);
    }
}
