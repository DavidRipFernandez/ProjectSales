namespace projectSales.Server.Domain.Entities;

public class Material
{
    public int MaterialId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public string? Sku { get; set; }

    public int? CategoriaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public CategoriaMaterial? Categoria { get; set; }

    public ICollection<PrecioTarifa> PreciosTarifas { get; set; } = new HashSet<PrecioTarifa>();
}
