namespace projectSales.Server.Contracts.Sessions;

public record SessionDto(int Id, DateTime CreatedAt, DateTime? LastSeenAt, DateTime ExpiresAt, bool Revoked, string? IpAddress, string? UserAgent, string? DeviceName, string? Location);
