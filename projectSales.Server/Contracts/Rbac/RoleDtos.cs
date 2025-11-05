namespace projectSales.Server.Contracts.Rbac;

public record RoleDto(int Id, string Nombre, string? Descripcion, bool Activo);

public record RoleRequest(string Nombre, string? Descripcion, bool Activo);
