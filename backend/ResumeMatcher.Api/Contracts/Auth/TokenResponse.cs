public record TokenResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public int ExpiresIn { get; init; }
    public string Email { get; init; } = default!;
    public string FullName { get; init; } = default!;
}