namespace projectSales.Server.Contracts.Catalogs;

public record MarcaDto(int MarcaId, string Nombre, string? Descripcion, bool Estado);
public record MarcaRequest(string Nombre, string? Descripcion, bool Estado);

public record CategoriaDto(int CategoriaId, string Nombre, string? Descripcion, bool Estado);
public record CategoriaRequest(string Nombre, string? Descripcion, bool Estado);

public record MaterialDto(int MaterialId, string Codigo, string Nombre, string? Descripcion, string UnidadMedida, string? Sku, int? CategoriaId, bool Estado, string? CategoriaNombre);
public record MaterialRequest(string Codigo, string Nombre, string? Descripcion, string UnidadMedida, string? Sku, int? CategoriaId, bool Estado);

public record ProveedorDto(string ProveedorCifId, string Nombre, string DomicilioSocial, bool Estado);
public record ProveedorRequest(string ProveedorCifId, string Nombre, string DomicilioSocial, bool Estado);

public record ContactoDto(int ContactoId, string Nombre, string? Correo, string Telefono, string? Descripcion, bool Estado);
public record ContactoRequest(string Nombre, string? Correo, string Telefono, string? Descripcion, bool Estado);

public record ProveedorMarcaDto(string ProveedorCifId, int MarcaId, bool Estado, string MarcaNombre);
public record ProveedorMarcaRequest(string ProveedorCifId, int MarcaId, bool Estado);

public record PrecioTarifaDto(int MaterialId, string MaterialCodigo, string MaterialNombre, string ProveedorCifId, string ProveedorNombre, int MarcaId, string MarcaNombre, decimal Precio, bool Estado);
public record PrecioTarifaRequest(int MaterialId, string ProveedorCifId, int MarcaId, decimal Precio, bool Estado);
