namespace projectSales.Server.Domain.Entities;

public class Module
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<ModuleAction> Actions { get; set; } = new HashSet<ModuleAction>();

    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}
