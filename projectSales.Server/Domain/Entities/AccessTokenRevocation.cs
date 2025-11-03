namespace projectSales.Server.Domain.Entities;

public class AccessTokenRevocation
{
    public int Id { get; set; }

    public string Jti { get; set; } = null!;

    public int? SessionId { get; set; }

    public DateTime RevokedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string? Reason { get; set; }

    public UserSession? Session { get; set; }
}
