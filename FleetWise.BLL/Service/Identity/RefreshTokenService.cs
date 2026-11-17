using System.Security.Cryptography;
using System.Text;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Identity;

public class RefreshTokenService(FleetWiseDbContext dbContext)
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7);

    public async Task<(string RawToken, DateTime ExpiresAtUtc)> CreateAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var rawToken = GenerateRawToken();
        var nowUtc = DateTime.UtcNow;
        var expiresAtUtc = nowUtc.Add(TokenLifetime);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = HashToken(rawToken),
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = expiresAtUtc,
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (rawToken, expiresAtUtc);
    }

    public async Task<(string RawToken, DateTime ExpiresAtUtc, ApplicationUser User)?> RotateAsync(
        string rawToken,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return null;
        }

        var tokenHash = HashToken(rawToken);

        var storedToken = await dbContext
            .RefreshTokens.Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        var nowUtc = DateTime.UtcNow;

        if (
            storedToken is null
            || storedToken.RevokedAtUtc is not null
            || storedToken.ExpiresAtUtc <= nowUtc
            || !storedToken.User.IsActive
        )
        {
            return null;
        }

        var newRawToken = GenerateRawToken();
        var newExpiresAtUtc = nowUtc.Add(TokenLifetime);
        var newTokenHash = HashToken(newRawToken);

        storedToken.RevokedAtUtc = nowUtc;
        storedToken.ReplacedByTokenHash = newTokenHash;

        dbContext.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = storedToken.UserId,
                TokenHash = newTokenHash,
                CreatedAtUtc = nowUtc,
                ExpiresAtUtc = newExpiresAtUtc,
            }
        );

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }

        return (newRawToken, newExpiresAtUtc, storedToken.User);
    }

    public async Task RevokeAsync(string rawToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return;
        }

        var tokenHash = HashToken(rawToken);

        var storedToken = await dbContext.RefreshTokens.SingleOrDefaultAsync(
            token => token.TokenHash == tokenHash,
            cancellationToken
        );

        if (storedToken is null || storedToken.RevokedAtUtc is not null)
        {
            return;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // التوكن تدوّر أو أُلغي بطلب متزامن.
        }
    }

    private static string GenerateRawToken()
    {
        return Convert
            .ToBase64String(RandomNumberGenerator.GetBytes(64))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
