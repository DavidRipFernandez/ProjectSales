using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectSales.Server.Domain.Entities;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Services.Auth;

public record SessionMetadata(string? IpAddress, string? UserAgent, string? DeviceName, string? Location);

public interface ISessionService
{
    Task<UserSession> CreateSessionAsync(int userId, SessionMetadata metadata, CancellationToken cancellationToken);
    Task TouchAsync(int sessionId, CancellationToken cancellationToken);
    Task RevokeAsync(int sessionId, string reason, CancellationToken cancellationToken);
    Task<List<UserSession>> GetActiveSessionsAsync(int userId, CancellationToken cancellationToken);
}

public class SessionService : ISessionService
{
    private readonly SalesystemDbContext _dbContext;
    private readonly JwtSettings _settings;

    public SessionService(SalesystemDbContext dbContext, IOptions<JwtSettings> options)
    {
        _dbContext = dbContext;
        _settings = options.Value;
    }

    public async Task<UserSession> CreateSessionAsync(int userId, SessionMetadata metadata, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var session = new UserSession
        {
            UserId = userId,
            CreatedAt = now,
            LastSeenAt = now,
            IpAddress = metadata.IpAddress,
            UserAgent = metadata.UserAgent,
            DeviceName = metadata.DeviceName,
            Location = metadata.Location,
            ExpiresAt = now.AddDays(_settings.RefreshTokenDays)
        };

        _dbContext.UserSessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task TouchAsync(int sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.UserSessions.FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        if (session is null)
        {
            return;
        }

        session.LastSeenAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(int sessionId, string reason, CancellationToken cancellationToken)
    {
        var session = await _dbContext.UserSessions.FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        if (session is null)
        {
            return;
        }

        session.RevokedAt = DateTime.UtcNow;
        session.RevokeReason = reason;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<UserSession>> GetActiveSessionsAsync(int userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > now)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
