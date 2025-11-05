using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Auth;
using projectSales.Server.Infrastructure.Persistence;
using projectSales.Server.Services.Auth;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IPermissionService _permissionService;

    public AuthController(
        SalesystemDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IPermissionService permissionService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _permissionService = permissionService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResult>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Activo &&
                                      (u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail),
                cancellationToken);

        if (user is null)
        {
            return Unauthorized(ApiResponse<LoginResult>.Fail("Credenciales inválidas", code: 401));
        }

        var hash = _passwordHasher.HashPassword(request.Password);
        if (!string.Equals(hash, user.PasswordHash, StringComparison.Ordinal))
        {
            return Unauthorized(ApiResponse<LoginResult>.Fail("Credenciales inválidas", code: 401));
        }

        var metadata = new SessionMetadata(
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString(),
            null,
            null);

        var session = await _sessionService.CreateSessionAsync(user.Id, metadata, cancellationToken);

        user.UltimoLoginUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var (refreshEntity, refreshTokenValue) = await _refreshTokenService.CreateTokenAsync(user, session, cancellationToken);
        _refreshTokenService.AppendRefreshCookie(Response, refreshTokenValue, refreshEntity.ExpiresAt);

        var roles = await _permissionService.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        var permissions = await _permissionService.GetPermissionsByModuleAsync(user.Id, cancellationToken);
        var accessToken = _tokenService.CreateAccessToken(user, session.Id, roles);

        var response = new LoginResult(
            accessToken,
            new UserSummary(user.Id, user.Username, user.Email),
            roles,
            permissions);

        return Ok(ApiResponse<LoginResult>.Ok(response));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<RefreshResult>>> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
        {
            return Unauthorized(ApiResponse<RefreshResult>.Fail("Refresh token requerido", code: 401));
        }

        var existing = await _refreshTokenService.FindByTokenAsync(tokenValue, cancellationToken);
        if (existing is null)
        {
            _refreshTokenService.RemoveRefreshCookie(Response);
            return Unauthorized(ApiResponse<RefreshResult>.Fail("Refresh token inválido", code: 401));
        }

        if (existing.RevokedAt.HasValue)
        {
            await _sessionService.RevokeAsync(existing.SessionId, "Reutilización de refresh token", cancellationToken);
            _refreshTokenService.RemoveRefreshCookie(Response);
            return Unauthorized(ApiResponse<RefreshResult>.Fail("Refresh token revocado", code: 401));
        }

        if (existing.ExpiresAt <= DateTime.UtcNow)
        {
            existing.RevokedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _refreshTokenService.RemoveRefreshCookie(Response);
            return Unauthorized(ApiResponse<RefreshResult>.Fail("Refresh token expirado", code: 401));
        }

        if (existing.Session.RevokedAt.HasValue || existing.Session.ExpiresAt <= DateTime.UtcNow)
        {
            await _sessionService.RevokeAsync(existing.SessionId, "Sesión expirada", cancellationToken);
            _refreshTokenService.RemoveRefreshCookie(Response);
            return Unauthorized(ApiResponse<RefreshResult>.Fail("Sesión no válida", code: 401));
        }

        var (rotatedEntity, rotatedValue) = await _refreshTokenService.RotateTokenAsync(existing, cancellationToken);
        await _sessionService.TouchAsync(existing.SessionId, cancellationToken);
        _refreshTokenService.AppendRefreshCookie(Response, rotatedValue, rotatedEntity.ExpiresAt);

        var roles = await _permissionService.GetRoleNamesForUserAsync(existing.UserId, cancellationToken);
        var accessToken = _tokenService.CreateAccessToken(existing.User, existing.SessionId, roles);

        return Ok(ApiResponse<RefreshResult>.Ok(new RefreshResult(accessToken)));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> Logout(CancellationToken cancellationToken)
    {
        int? sessionId = null;
        var sessionClaim = User.FindFirst("sid");
        if (sessionClaim != null && int.TryParse(sessionClaim.Value, out var parsedSessionId))
        {
            sessionId = parsedSessionId;
        }

        if (Request.Cookies.TryGetValue("refresh_token", out var tokenValue) && !string.IsNullOrWhiteSpace(tokenValue))
        {
            var existing = await _refreshTokenService.FindByTokenAsync(tokenValue, cancellationToken);
            if (existing is not null)
            {
                existing.RevokedAt = DateTime.UtcNow;
            }
        }

        if (sessionId.HasValue)
        {
            await _sessionService.RevokeAsync(sessionId.Value, "Logout", cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        _refreshTokenService.RemoveRefreshCookie(Response);
        return Ok(ApiResponse<string>.Ok("Sesión cerrada"));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LoginResult>>> Me(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue("sub");
        if (userIdValue is null || !int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(ApiResponse<LoginResult>.Fail("Usuario no válido", code: 401));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Unauthorized(ApiResponse<LoginResult>.Fail("Usuario no válido", code: 401));
        }

        var roles = await _permissionService.GetRoleNamesForUserAsync(userId, cancellationToken);
        var permissions = await _permissionService.GetPermissionsByModuleAsync(userId, cancellationToken);

        var result = new LoginResult(
            string.Empty,
            new UserSummary(user.Id, user.Username, user.Email),
            roles,
            permissions);

        return Ok(ApiResponse<LoginResult>.Ok(result));
    }
}
