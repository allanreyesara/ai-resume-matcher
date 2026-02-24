using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Api.Contracts.Auth;
using ResumeMatcher.Api.Infrastructure.Data;
using ResumeMatcher.Api.Infrastructure.Data.Auth;
using ResumeMatcher.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using ResumeMatcher.Api.Infrastructure.Jobs;
using System.Security.Claims;

namespace ResumeMatcher.Api.Controllers;

[ApiController]
[Route("jobs")]

public class JobController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMatchService _matchService;

    public JobController(ApplicationDbContext db, IMatchService matchService){
        _db = db;
        _matchService = matchService;

    }
    [HttpPost("/match")]
    public async Task<IActionResult> JobMatch([FromBody] JobMatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.JobText))
        {
            return BadRequest("Job text is required");
        }
        var userId = GetUserIdFromJwt();

        var matches = await _matchService.MatchAsync(
            userId, 
            request.DocumentId,
            request.JobText,
            request.TopK,
            request.useLlm
        );

        return Ok(matches);
    }

    private Guid GetUserIdFromJwt()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("userId");

        if (userIdClaim == null) throw new UnauthorizedAccessException("User ID not found in token");
        if (!Guid.TryParse(userIdClaim.Value, out var userId)) throw new UnauthorizedAccessException("Invalid User ID in token");

        return userId;
    }


}