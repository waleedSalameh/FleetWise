using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/admin/users")]
public sealed class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    [Authorize(Policy = "Permission:Users.View")]
    [HttpGet]
    public async Task<ActionResult<List<UserSummaryDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var users = await userManagementService.GetAllAsync(
                pageNumber,
                pageSize,
                cancellationToken
            );
            return Ok(users);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Permission:Users.Create")]
    [HttpPost]
    public async Task<ActionResult<UserSummaryDto>> Create(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorUserId = GetActorUserId();
        if (actorUserId is null)
        {
            return Forbid();
        }

        try
        {
            var created = await userManagementService.CreateAsync(actorUserId, dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Permission:Users.Disable")]
    [HttpPost("{userId}/disable")]
    public Task<IActionResult> Disable(string userId, CancellationToken cancellationToken) =>
        SetActive(userId, false, cancellationToken);

    [Authorize(Policy = "Permission:Users.Update")]
    [HttpPost("{userId}/activate")]
    public Task<IActionResult> Activate(string userId, CancellationToken cancellationToken) =>
        SetActive(userId, true, cancellationToken);

    [Authorize(Policy = "Permission:Users.Update")]
    [HttpPut("{userId}/password")]
    public async Task<IActionResult> ResetPassword(
        string userId,
        [FromBody] ResetUserPasswordDto dto,
        CancellationToken cancellationToken
    )
    {
        var actorUserId = GetActorUserId();
        if (actorUserId is null)
        {
            return Forbid();
        }

        try
        {
            await userManagementService.ResetPasswordAsync(actorUserId, userId, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User was not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task<IActionResult> SetActive(
        string userId,
        bool isActive,
        CancellationToken cancellationToken
    )
    {
        var actorUserId = GetActorUserId();
        if (actorUserId is null)
        {
            return Forbid();
        }

        try
        {
            await userManagementService.SetActiveAsync(actorUserId, userId, isActive, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User was not found." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
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
