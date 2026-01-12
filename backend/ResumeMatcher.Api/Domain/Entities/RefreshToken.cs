using System.ComponentModel.DataAnnotations;

namespace ResumeMatcher.Api.Domain.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(200)]
    public string HashedToken { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    [MaxLength(200)]
    public string? CreatedByIp { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; set; } = default!;
}