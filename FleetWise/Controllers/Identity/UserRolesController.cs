using System.Security.Claims;
using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/admin/users/{userId}/roles")]
[Authorize(Policy = "Permission:Roles.Manage")]
public sealed class UserRolesController(IRoleManagementService roleManagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoleSummaryDto>>> GetUserRoles(
        string userId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return Ok(await roleManagementService.GetUserRolesAsync(userId, cancellationToken));
        }
        catch (KeyNotFoundException) { return NotFound(new { message = "User was not found." }); }
    }

    [HttpPut]
    public async Task<IActionResult> ReplaceUserRoles(
        string userId,
        [FromBody] ReplaceUserRolesDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (actorId is null) return Forbid();

        try
        {
            await roleManagementService.ReplaceUserRolesAsync(actorId, userId, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(new { message = "User or role was not found." }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}
