namespace projectSales.Server.Contracts.Rbac;

public record UserDto(int Id, string Username, string Email, bool Activo, IEnumerable<string> Roles);

public record CreateUserRequest(string Username, string Email, string Password, bool Activo, IEnumerable<int> RoleIds);

public record UpdateUserRequest(string Username, string Email, bool Activo, string? Password, IEnumerable<int> RoleIds);
