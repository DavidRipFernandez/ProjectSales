namespace projectSales.Server.Contracts.Auth;

public record LoginRequest(string UsernameOrEmail, string Password);

public record LoginResult(string AccessToken, UserSummary User, IEnumerable<string> Roles, IDictionary<string, List<string>> PermissionsByModule);

public record RefreshResult(string AccessToken);

public record UserSummary(int Id, string Username, string Email);
