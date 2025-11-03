namespace projectSales.Server.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SessionId { get; set; }

    public byte[] TokenHash { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public int? RotatedFromTokenId { get; set; }

    public int? ReplacedByTokenId { get; set; }

    public User User { get; set; } = null!;

    public UserSession Session { get; set; } = null!;

    public RefreshToken? RotatedFromToken { get; set; }

    public RefreshToken? ReplacedByToken { get; set; }

    public ICollection<RefreshToken> RotatedTokens { get; set; } = new HashSet<RefreshToken>();

    public ICollection<RefreshToken> ReplacedTokens { get; set; } = new HashSet<RefreshToken>();
}
