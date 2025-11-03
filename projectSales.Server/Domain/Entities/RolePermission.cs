namespace projectSales.Server.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int ModuleId { get; set; }

    public int ActionId { get; set; }

    public bool Allowed { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Role Role { get; set; } = null!;

    public Module Module { get; set; } = null!;

    public ModuleAction Action { get; set; } = null!;
}
