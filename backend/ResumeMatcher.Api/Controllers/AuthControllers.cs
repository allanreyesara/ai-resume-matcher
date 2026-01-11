using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Contracts.Auth;
using ResumeMatcher.Api.Infrastructure.Data;
using ResumeMatcher.Api.Infrastructure.Data.Auth;
using ResumeMatcher.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

    //Register new user

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


    //Login existing user
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var email = (request.Email ?? "").Trim().ToLowerInvariant();
        var password = request.Password ?? "";

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !_passwordService.VerifyPassword(user, password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _jwtService.CreateToken(user);
        return Ok(new { email = user.Email, fullName = user.FullName, token });
        
    }


    //Get current user info
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
    Guid userId;

    try
    {
        userId = User.GetUserId();
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(ex.Message);
    }

    var user = await _db.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
        return Unauthorized("User not found.");


    return Ok(new
    {
        user.Id,
        user.Email,
        user.FullName,
        user.CreatedAt
    });
}

    //Logout user (for JWT, this is typically handled on the client side)

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT is stateless: logout is handled client-side
        // Client must delete the token

        return NoContent(); // 204
    }
}