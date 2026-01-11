namespace ResumeMatcher.Api.Contracts.Auth;

public record LoginRequest(string Email, string Password, string Token);