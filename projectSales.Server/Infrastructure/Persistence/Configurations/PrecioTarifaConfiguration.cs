using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class PrecioTarifaConfiguration : IEntityTypeConfiguration<PrecioTarifa>
{
    public void Configure(EntityTypeBuilder<PrecioTarifa> builder)
    {
        builder.ToTable("PreciosTarifas", "dbo", tb =>
        {
            tb.HasCheckConstraint("CK_PreciosTarifas_Precio", "[Precio] >= 0");
        });

        builder.HasKey(e => new { e.MaterialId, e.ProveedorCifId, e.MarcaId }).HasName("PK_PreciosTarifas");

        builder.Property(e => e.Precio)
            .HasColumnName("Precio")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(e => e.MaterialId).HasColumnName("MaterialId");
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

        builder.HasIndex(e => e.ProveedorCifId)
            .HasDatabaseName("IX_PreciosTarifas_Prov");

        builder.HasIndex(e => e.MarcaId)
            .HasDatabaseName("IX_PreciosTarifas_Marca");

        builder.HasOne(e => e.Material)
            .WithMany(e => e.PreciosTarifas)
            .HasForeignKey(e => e.MaterialId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PreciosTarifas_Material");

        builder.HasOne(e => e.Proveedor)
            .WithMany(e => e.PreciosTarifas)
            .HasForeignKey(e => e.ProveedorCifId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PreciosTarifas_Proveedor");

        builder.HasOne(e => e.Marca)
            .WithMany(e => e.PreciosTarifas)
            .HasForeignKey(e => e.MarcaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PreciosTarifas_Marca");
    }
}
