namespace projectSales.Server.Domain.Entities;

public class PrecioTarifa
{
    public decimal Precio { get; set; }

    public int MaterialId { get; set; }

    public string ProveedorCifId { get; set; } = null!;

    public int MarcaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public Material Material { get; set; } = null!;

    public Proveedor Proveedor { get; set; } = null!;

    public Marca Marca { get; set; } = null!;
}
