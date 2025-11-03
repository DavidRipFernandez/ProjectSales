namespace projectSales.Server.Domain.Entities;

public class Proveedor
{
    public string ProveedorCifId { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string DomicilioSocial { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? CreadoPor { get; set; }

    public int? ModificadoPor { get; set; }

    public bool Estado { get; set; }

    public ICollection<Contacto> Contactos { get; set; } = new HashSet<Contacto>();

    public ICollection<ProveedorMarca> ProveedoresMarcas { get; set; } = new HashSet<ProveedorMarca>();

    public ICollection<PrecioTarifa> PreciosTarifas { get; set; } = new HashSet<PrecioTarifa>();
}
