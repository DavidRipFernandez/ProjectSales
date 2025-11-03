namespace projectSales.Server.Domain.Entities;

public class UserRole
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? ModificadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
