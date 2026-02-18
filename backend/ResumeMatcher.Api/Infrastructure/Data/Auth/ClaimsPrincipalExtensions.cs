using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ResumeMatcher.Api.Infrastructure.Data.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Missing user id claim.");

        if (!Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAccessException("Invalid user id claim format.");

        return userId;
    }
}