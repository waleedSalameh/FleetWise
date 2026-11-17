using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/admin/users/{userId}/permissions")]
public sealed class UserPermissionsController(
    IUserPermissionManagementService permissionManagementService
) : ControllerBase
{
    [Authorize(Policy = "Permission:Permissions.Manage")]
    [HttpGet]
    public async Task<ActionResult<List<UserPermissionStatusDto>>> GetUserPermissions(
        string userId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var permissions = await permissionManagementService.GetUserPermissionsAsync(
                userId,
                cancellationToken
            );

            return Ok(permissions);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Active user was not found." });
        }
    }

    [Authorize(Policy = "Permission:Permissions.Manage")]
    [HttpPut]
    public async Task<IActionResult> SetOverride(
        string userId,
        [FromBody] SetUserPermissionOverrideDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorUserId = GetActorUserId();
        if (actorUserId is null) return Forbid();

        try
        {
            await permissionManagementService.SetUserOverrideAsync(
                actorUserId,
                userId,
                dto,
                cancellationToken
            );

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Active user was not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException)
        {
            return Forbid();
        }
    }

    [Authorize(Policy = "Permission:Permissions.Manage")]
    [HttpDelete("{permissionCode}")]
    public async Task<IActionResult> RemoveOverride(
        string userId,
        string permissionCode,
        CancellationToken cancellationToken
    )
    {
        var actorUserId = GetActorUserId();
        if (actorUserId is null) return Forbid();

        try
        {
            var removed = await permissionManagementService.RemoveUserOverrideAsync(
                actorUserId,
                userId,
                permissionCode,
                cancellationToken
            );

            if (!removed)
            {
                return NotFound(new { message = "User permission override was not found." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Active user was not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private string? GetActorUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");
}
