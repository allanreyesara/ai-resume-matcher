using Microsoft.AspNetCore.Identity;
using ResumeMatcher.Api.Domain.Entities;

namespace ResumeMatcher.Api.Infrastructure.Data.Auth;
public interface IPasswordService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password, string hashedPassword);
}

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
     => _hasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string password, string hashedPassword)
    => _hasher.VerifyHashedPassword(user, hashedPassword, password) == PasswordVerificationResult.Success;
}