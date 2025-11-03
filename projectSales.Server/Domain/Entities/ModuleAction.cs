namespace projectSales.Server.Domain.Entities;

public class ModuleAction
{
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Module Module { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}
