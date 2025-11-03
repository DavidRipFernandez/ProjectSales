namespace projectSales.Server.Domain.Entities;

public class CategoriaMaterial
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public ICollection<Material> Materiales { get; set; } = new HashSet<Material>();
}
