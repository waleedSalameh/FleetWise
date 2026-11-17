using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/admin/audit")]
[Authorize(Policy = "Permission:Permissions.Manage")]
public sealed class AdminAuditController(IAdminAuditService auditService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminAuditEntryDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok(await auditService.GetAllAsync(pageNumber, pageSize, cancellationToken));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
