using FleetWise.BLL.DTOs.Identity;
using FleetWise.BLL.Service.Identity;
using FleetWise.DAL.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FleetWise.API.Controllers.Identity;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string TrustedFrontendOrigin = "http://localhost:4200";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService,
        RefreshTokenService refreshTokenService
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthFivePerMinute")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true
        );

        if (!signInResult.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = _jwtTokenService.CreateAccessToken(
            user.Id,
            user.Email ?? request.Email,
            roles,
            user.SecurityStamp ?? string.Empty
        );

        var (refreshTokenValue, refreshTokenExpiresAtUtc) = await _refreshTokenService.CreateAsync(
            user.Id
        );

        Response.Cookies.Append(
            "refreshToken",
            refreshTokenValue,
            CreateRefreshTokenCookieOptions(refreshTokenExpiresAtUtc)
        );

        return Ok(response);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("AuthFivePerMinute")]
    public async Task<ActionResult<LoginResponseDto>> Refresh(CancellationToken cancellationToken)
    {
        var requestOrigin = Request.Headers["Origin"].ToString();

        if (
            !string.Equals(requestOrigin, TrustedFrontendOrigin, StringComparison.OrdinalIgnoreCase)
        )
        {
            return Forbid();
        }

        var currentRefreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(currentRefreshToken))
        {
            return Unauthorized(new { message = "Refresh token is invalid." });
        }

        var rotation = await _refreshTokenService.RotateAsync(
            currentRefreshToken,
            cancellationToken
        );

        if (rotation is null)
        {
            return Unauthorized(new { message = "Refresh token is invalid." });
        }

        var (newRefreshToken, refreshTokenExpiresAtUtc, user) = rotation.Value;
        var roles = await _userManager.GetRolesAsync(user);

        var response = _jwtTokenService.CreateAccessToken(
            user.Id,
            user.Email ?? string.Empty,
            roles,
            user.SecurityStamp ?? string.Empty
        );

        Response.Cookies.Append(
            "refreshToken",
            newRefreshToken,
            CreateRefreshTokenCookieOptions(refreshTokenExpiresAtUtc)
        );

        return Ok(response);
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(DateTime expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = new DateTimeOffset(expiresAtUtc),
            Path = "/api/auth",
        };
    }

    [HttpPost("logout")]
    [EnableRateLimiting("AuthFivePerMinute")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var requestOrigin = Request.Headers["Origin"].ToString();

        if (
            !string.Equals(requestOrigin, TrustedFrontendOrigin, StringComparison.OrdinalIgnoreCase)
        )
        {
            return Forbid();
        }

        var refreshToken = Request.Cookies["refreshToken"];

        await _refreshTokenService.RevokeAsync(refreshToken ?? string.Empty, cancellationToken);

        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/auth",
            }
        );

        return NoContent();
    }
}
