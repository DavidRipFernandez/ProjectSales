using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectSales.Server.Domain.Entities;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Services.Auth;

public interface IRefreshTokenService
{
    Task<(RefreshToken Entity, string Token)> CreateTokenAsync(User user, UserSession session, CancellationToken cancellationToken);
    Task<(RefreshToken Entity, string Token)> RotateTokenAsync(RefreshToken currentToken, CancellationToken cancellationToken);
    Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken);
    byte[] HashToken(string token);
    void AppendRefreshCookie(HttpResponse response, string token, DateTime expiresAt);
    void RemoveRefreshCookie(HttpResponse response);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly SalesystemDbContext _dbContext;
    private readonly JwtSettings _settings;

    public RefreshTokenService(SalesystemDbContext dbContext, IOptions<JwtSettings> options)
    {
        _dbContext = dbContext;
        _settings = options.Value;
    }

    public async Task<(RefreshToken Entity, string Token)> CreateTokenAsync(User user, UserSession session, CancellationToken cancellationToken)
    {
        var (entity, token) = BuildNewToken(user, session);
        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (entity, token);
    }

    public async Task<(RefreshToken Entity, string Token)> RotateTokenAsync(RefreshToken currentToken, CancellationToken cancellationToken)
    {
        var session = currentToken.Session;
        var user = currentToken.User;
        var (newEntity, tokenValue) = BuildNewToken(user, session);

        currentToken.RevokedAt = DateTime.UtcNow;
        currentToken.ReplacedByToken = newEntity;
        newEntity.RotatedFromToken = currentToken;
        newEntity.RotatedFromTokenId = currentToken.Id;

        _dbContext.RefreshTokens.Add(newEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (newEntity, tokenValue);
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken)
    {
        var hash = HashToken(token);
        return await _dbContext.RefreshTokens
            .Include(t => t.User)
            .Include(t => t.Session)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
    }

    public byte[] HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token + _settings.Pepper);
        return sha.ComputeHash(bytes);
    }

    public void AppendRefreshCookie(HttpResponse response, string token, DateTime expiresAt)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/auth/refresh",
            Expires = expiresAt
        };
        response.Cookies.Append("refresh_token", token, options);
    }

    public void RemoveRefreshCookie(HttpResponse response)
    {
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/auth/refresh",
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = true
        });
    }

    private (RefreshToken Entity, string Token) BuildNewToken(User user, UserSession session)
    {
        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var now = DateTime.UtcNow;
        var entity = new RefreshToken
        {
            UserId = user.Id,
            SessionId = session.Id,
            User = user,
            Session = session,
            TokenHash = HashToken(value),
            CreatedAt = now,
            ExpiresAt = now.AddDays(_settings.RefreshTokenDays)
        };
        return (entity, value);
    }
}
