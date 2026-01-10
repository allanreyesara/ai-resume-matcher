using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Contracts.Auth;
using ResumeMatcher.Api.Infrastructure.Data;
using ResumeMatcher.Api.Infrastructure.Data.Auth;
using ResumeMatcher.Api.Domain.Entities;

namespace ResumeMatcher.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthController(
        ApplicationDbContext db,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var email = (request.Email ?? "").Trim().ToLowerInvariant();
        var password = request.Password ?? "";
        var fullName = (request.FullName ?? "").Trim();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            return BadRequest("Email, password and full name are required.");
        }

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            return Conflict("Email is already registered.");
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true 
        };

        user.PasswordHash = _passwordService.HashPassword(user, password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwtService.CreateToken(user);

        return Ok(new { email, fullName, token });
    }
}






