using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FleetWise.BLL.DTOs.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FleetWise.BLL.Service.Identity;

public sealed class JwtTokenService(IConfiguration configuration)
{
    public LoginResponseDto CreateAccessToken(
        string userId,
        string email,
        IEnumerable<string> roles,
        string securityStamp
    )
    {
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        var signingKeyBase64 = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("JWT SigningKey is missing.");

        var accessTokenMinutesValue = configuration["Jwt:AccessTokenMinutes"];

        if (
            !int.TryParse(accessTokenMinutesValue, out var accessTokenMinutes)
            || accessTokenMinutes <= 0
        )
        {
            throw new InvalidOperationException(
                "JWT AccessTokenMinutes must be a positive number."
            );
        }

        byte[] signingKeyBytes;

        try
        {
            signingKeyBytes = Convert.FromBase64String(signingKeyBase64);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("JWT SigningKey must be a valid Base64 value.");
        }

        if (signingKeyBytes.Length < 32)
        {
            throw new InvalidOperationException("JWT SigningKey must contain at least 32 bytes.");
        }

        var nowUtc = DateTime.UtcNow;
        var expiresAtUtc = nowUtc.AddMinutes(accessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("security_stamp", securityStamp),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(signingKeyBytes),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expiresAtUtc,
            signingCredentials: credentials
        );

        return new LoginResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
        };
    }
}
