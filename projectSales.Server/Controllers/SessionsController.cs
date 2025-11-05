using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Sessions;
using projectSales.Server.Infrastructure.Persistence;
using projectSales.Server.Services.Auth;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("sessions")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;
    private readonly ISessionService _sessionService;

    public SessionsController(SalesystemDbContext dbContext, ISessionService sessionService)
    {
        _dbContext = dbContext;
        _sessionService = sessionService;
    }

    [HttpGet("mine")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionDto>>>> GetMySessions(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<IEnumerable<SessionDto>>.Fail("Usuario no autorizado", code: 401));
        }

        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = sessions.Select(s => new SessionDto(s.Id, s.CreatedAt, s.LastSeenAt, s.ExpiresAt, s.RevokedAt != null, s.IpAddress, s.UserAgent, s.DeviceName, s.Location));
        return Ok(ApiResponse<IEnumerable<SessionDto>>.Ok(result));
    }

    [HttpPost("{id:int}/revoke")]
    public async Task<ActionResult<ApiResponse<string>>> RevokeSession(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(ApiResponse<string>.Fail("Usuario no autorizado", code: 401));
        }

        var session = await _dbContext.UserSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, cancellationToken);
        if (session is null)
        {
            return NotFound(ApiResponse<string>.Fail("Sesión no encontrada", code: 404));
        }

        await _sessionService.RevokeAsync(session.Id, "Revocada por el usuario", cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Sesión revocada"));
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdValue is null || !int.TryParse(userIdValue, out var userId))
        {
            return null;
        }
        return userId;
    }
}
