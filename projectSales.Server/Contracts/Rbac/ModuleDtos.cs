namespace projectSales.Server.Contracts.Rbac;

public record ModuleActionDto(int Id, string Key, string Name, int SortOrder, bool IsActive);

public record ModuleDto(int Id, string Key, string Name, bool IsActive, IEnumerable<ModuleActionDto> Actions);
