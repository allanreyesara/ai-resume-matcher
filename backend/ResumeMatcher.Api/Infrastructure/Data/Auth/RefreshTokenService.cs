using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResumeMatcher.Api.Domain.Entities;

namespace ResumeMatcher.Api.Infrastructure.Data.Auth;

public record RefreshTokenIssueResult(string RefreshTokenPlain, RefreshToken Entity);

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(Guid userId, string? ip, string? userAgent, CancellationToken ct);
    Task<(bool Ok, Guid UserId, string? Error)> ValidateAsync(string refreshTokenPlain, CancellationToken ct);
    Task<(bool Ok, string NewRefreshTokenPlain, string? Error)> RotateAsync(string refreshTokenPlain, string? ip, string? userAgent, CancellationToken ct);
    Task<bool> RevokeAsync(string refreshTokenPlain, CancellationToken ct);
    Task<int> RevokeAllAsync(Guid userId, CancellationToken ct);
    
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _db;
    private readonly RefreshTokenOptions _rto;

    public RefreshTokenService(ApplicationDbContext db, IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        _db = db;
        _rto = refreshTokenOptions.Value;
    }

    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Base64UrlEncode(bytes);
    }

    private static string HashToken(string plain)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(plain);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
    public async Task<RefreshTokenIssueResult> IssueAsync(Guid userId, string? ip, string? userAgent, CancellationToken ct)
    {
        var plain = GenerateSecureToken();
        var hashed = HashToken(plain);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HashedToken = hashed,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_rto.ExpirationDays),
            CreatedByIp = ip,
            UserAgent = userAgent
        };

        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(ct);

        return new RefreshTokenIssueResult(plain, token);
    }

    public async Task<bool> RevokeAsync(string refreshTokenPlain, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenPlain))
        {
            return false;
        }

        var hashed = HashToken(refreshTokenPlain);
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.HashedToken == hashed, ct);

        if (token == null )
        {
            return false;
        }

        if (token.IsRevoked || token.IsExpired)
        {
            return true;
        }

        token.RevokedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<(bool Ok, Guid UserId, string? Error)> ValidateAsync(string refreshTokenPlain, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            return (false, Guid.Empty, "Missing refresh token.");

        var hash = HashToken(refreshTokenPlain);

        var rt = await _db.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(x => x.HashedToken == hash, ct);

        if (rt is null) return (false, Guid.Empty, "Refresh token not found.");
        if (!rt.IsActive) return (false, Guid.Empty, "Refresh token is expired or revoked.");

        return (true, rt.UserId, null);
    }

    public async Task<(bool Ok, string NewRefreshTokenPlain, string? Error)> RotateAsync(
        string refreshTokenPlain, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            return (false, "", "Missing refresh token.");

        var oldHash = HashToken(refreshTokenPlain);

        var old = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.HashedToken == oldHash, ct);

        if (old is null) return (false, "", "Refresh token not found.");
        if (!old.IsActive) return (false, "", "Refresh token is expired or revoked.");

        var newPlain = GenerateSecureToken();
        var newHash = HashToken(newPlain);

        old.RevokedAtUtc = DateTime.UtcNow;
        old.ReplacedByTokenHash = newHash;

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = old.UserId,
            HashedToken = newHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(14),
            CreatedByIp = ip,
            UserAgent = userAgent
        };

        _db.RefreshTokens.Add(token);

        await _db.SaveChangesAsync(ct);
        return (true, newPlain, null);
    }

    public async Task<int> RevokeAllAsync(Guid userId, CancellationToken ct)
    {
        var tokens = await _db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && !t.IsExpired)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return tokens.Count;
    }

}