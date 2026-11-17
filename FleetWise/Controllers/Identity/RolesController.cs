using System.Security.Claims;
using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Policy = "Permission:Roles.Manage")]
public sealed class RolesController(IRoleManagementService roleManagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoleSummaryDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await roleManagementService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<RoleSummaryDto>> Create(
        [FromBody] CreateRoleDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorId = GetActorUserId();
        if (actorId is null) return Forbid();

        try
        {
            var role = await roleManagementService.CreateAsync(actorId, dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, role);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{roleId}")]
    public async Task<ActionResult<RoleSummaryDto>> Update(
        string roleId,
        [FromBody] UpdateRoleDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorId = GetActorUserId();
        if (actorId is null) return Forbid();

        try
        {
            return Ok(await roleManagementService.UpdateAsync(actorId, roleId, dto, cancellationToken));
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{roleId}")]
    public async Task<IActionResult> Delete(string roleId, CancellationToken cancellationToken)
    {
        var actorId = GetActorUserId();
        if (actorId is null) return Forbid();

        try
        {
            await roleManagementService.DeleteAsync(actorId, roleId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpGet("{roleId}/permissions")]
    public async Task<ActionResult<List<RolePermissionStatusDto>>> GetPermissions(
        string roleId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return Ok(await roleManagementService.GetRolePermissionsAsync(roleId, cancellationToken));
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{roleId}/permissions")]
    public async Task<IActionResult> ReplacePermissions(
        string roleId,
        [FromBody] ReplaceRolePermissionsDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorId = GetActorUserId();
        if (actorId is null) return Forbid();

        try
        {
            await roleManagementService.ReplaceRolePermissionsAsync(actorId, roleId, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    private string? GetActorUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
}
