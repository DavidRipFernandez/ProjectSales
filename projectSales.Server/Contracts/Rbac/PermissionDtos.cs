namespace projectSales.Server.Contracts.Rbac;

public record RolePermissionDto(int ModuleId, int ActionId, bool Allowed);

public record RolePermissionRequest(string ModuleKey, string ActionKey, bool Allowed);
