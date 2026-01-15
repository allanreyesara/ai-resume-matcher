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
    private readonly IRefreshTokenService _refresh;

    public AuthController(
        ApplicationDbContext db,
        IPasswordService passwordService,
        IJwtService jwtService,
        IRefreshTokenService refresh)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _refresh = refresh;
    }

    private const string RefreshCookieName = "refresh_token";

    private void SetRefreshCookie(string refreshToken)
    {
        Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(14),
            Path = "/"
        });
    }

    private void ClearRefreshCookie()
    {
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            Path = "/auth/refresh"
        });

         Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            Path = "/"
        });
    }

    private string? GetRefreshCookie()
    {
        return Request.Cookies.TryGetValue(RefreshCookieName, out var token) ? token : null;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
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

        var accessToken = _jwtService.CreateToken(user);
        var refreshToken = await _refresh.IssueAsync(
            userId: user.Id,
            userAgent: GetUserAgent(),
            ip: GetIp(),
            ct: cancellationToken); 
        
        SetRefreshCookie(refreshToken.RefreshTokenPlain);

        return Ok(new TokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = _jwtService.ExpirationSeconds,
            Email = user.Email,
            FullName = user.FullName
        });
        
    }

    //Refresh access token
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken cancellationToken)
    {
        var (isValid, userId, error) = await _refresh.ValidateAsync(req.RefreshToken, cancellationToken);
        if (!isValid)
        {
            return Unauthorized(error);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        var (rotValid, newRefreshPlain, rotEror) = await _refresh.RotateAsync(req.RefreshToken, GetIp(), GetUserAgent(), cancellationToken);
        if (!rotValid)
        {
            return Unauthorized(rotEror);
        }

        var accessToken = _jwtService.CreateToken(user);
        SetRefreshCookie(newRefreshPlain);

        return Ok(new TokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = _jwtService.ExpirationSeconds,
            Email = user.Email,
            FullName = user.FullName
        });
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

    //Logout user 
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = GetRefreshCookie();
        if (!string.IsNullOrWhiteSpace(refreshToken)) await _refresh.RevokeAsync(refreshToken, ct);
        ClearRefreshCookie();
        return NoContent();
    }

    private string? GetUserAgent() => Request.Headers.UserAgent.ToString();
    private string? GetIp() => HttpContext.Connection.RemoteIpAddress?.ToString();
}