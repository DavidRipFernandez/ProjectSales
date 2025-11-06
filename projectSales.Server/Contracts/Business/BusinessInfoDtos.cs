namespace projectSales.Server.Contracts.Business;

public record BusinessInfoDto(
    int Id,
    string EmpresaNombre,
    string? Nit,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail,
    string? Direccion,
    string? Ciudad,
    string? Pais,
    string? Web,
    string? Facebook,
    string? Instagram,
    string? Tiktok,
    bool IsPrimary);

public record UpdateBusinessInfoRequest(
    string EmpresaNombre,
    string? Nit,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail,
    string? Direccion,
    string? Ciudad,
    string? Pais,
    string? Web,
    string? Facebook,
    string? Instagram,
    string? Tiktok);
