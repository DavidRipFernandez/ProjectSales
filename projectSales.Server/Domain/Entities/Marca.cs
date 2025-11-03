namespace projectSales.Server.Domain.Entities;

public class Marca
{
    public int MarcaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public ICollection<ProveedorMarca> ProveedoresMarcas { get; set; } = new HashSet<ProveedorMarca>();

    public ICollection<PrecioTarifa> PreciosTarifas { get; set; } = new HashSet<PrecioTarifa>();
}
