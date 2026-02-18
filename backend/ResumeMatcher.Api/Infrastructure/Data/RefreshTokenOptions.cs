namespace ResumeMatcher.Api.Infrastructure.Data;

public class RefreshTokenOptions
{
    public int ExpirationDays { get; set; } = 14;
}