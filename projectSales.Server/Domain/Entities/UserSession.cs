namespace projectSales.Server.Domain.Entities;

public class UserSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceName { get; set; }

    public string? Location { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokeReason { get; set; }

    public User User { get; set; } = null!;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

    public ICollection<AccessTokenRevocation> AccessTokenRevocations { get; set; } = new HashSet<AccessTokenRevocation>();
}
