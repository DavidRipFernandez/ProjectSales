namespace projectSales.Server.Domain.Entities;

public class Contacto
{
    public int ContactoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    public string Telefono { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string ProveedorCifId { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public Proveedor Proveedor { get; set; } = null!;
}
