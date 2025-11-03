namespace projectSales.Server.Domain.Entities;

public class ProveedorMarca
{
    public string ProveedorCifId { get; set; } = null!;

    public int MarcaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public Proveedor Proveedor { get; set; } = null!;

    public Marca Marca { get; set; } = null!;
}
