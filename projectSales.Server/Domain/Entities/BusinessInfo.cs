namespace projectSales.Server.Domain.Entities;

public class BusinessInfo
{
    public int Id { get; set; }

    public string EmpresaNombre { get; set; } = null!;

    public string? Nit { get; set; }

    public string? ContactoNombre { get; set; }

    public string? ContactoTelefono { get; set; }

    public string? ContactoEmail { get; set; }

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public string? Pais { get; set; }

    public string? Web { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Tiktok { get; set; }

    public bool IsPrimary { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
