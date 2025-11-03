using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores", "dbo");

        builder.HasKey(e => e.ProveedorCifId).HasName("PK_Proveedores");

        builder.Property(e => e.ProveedorCifId).HasColumnName("ProveedorCifId").HasMaxLength(50);
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DomicilioSocial).HasColumnName("DomicilioSocial").HasMaxLength(100).IsRequired();
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2(7)");
        builder.Property(e => e.CreadoPor).HasColumnName("CreadoPor");
        builder.Property(e => e.ModificadoPor).HasColumnName("ModificadoPor");
        builder.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue(true);

        builder.HasIndex(e => e.Nombre)
            .HasDatabaseName("UX_Proveedores_Nombre")
            .IsUnique();
    }
}
