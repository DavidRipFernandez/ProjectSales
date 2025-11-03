namespace projectSales.Server.Domain.Entities;

public class Role
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}
