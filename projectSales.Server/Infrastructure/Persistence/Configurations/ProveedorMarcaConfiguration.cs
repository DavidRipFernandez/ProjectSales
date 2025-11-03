using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class ProveedorMarcaConfiguration : IEntityTypeConfiguration<ProveedorMarca>
{
    public void Configure(EntityTypeBuilder<ProveedorMarca> builder)
    {
        builder.ToTable("ProveedoresMarcas", "dbo");

        builder.HasKey(e => new { e.ProveedorCifId, e.MarcaId }).HasName("PK_ProveedoresMarcas");

        builder.Property(e => e.ProveedorCifId).HasColumnName("ProveedorCifId").HasMaxLength(50);
        builder.Property(e => e.MarcaId).HasColumnName("MarcaId");
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2(7)");
        builder.Property(e => e.CreadoPor).HasColumnName("CreadoPor");
        builder.Property(e => e.ModificadoPor).HasColumnName("ModificadoPor");
        builder.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue(true);

        builder.HasOne(e => e.Proveedor)
            .WithMany(e => e.ProveedoresMarcas)
            .HasForeignKey(e => e.ProveedorCifId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_ProvMarcas_Prov");

        builder.HasOne(e => e.Marca)
            .WithMany(e => e.ProveedoresMarcas)
            .HasForeignKey(e => e.MarcaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_ProvMarcas_Marca");
    }
}
