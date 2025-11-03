using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materiales", "dbo");

        builder.HasKey(e => e.MaterialId).HasName("PK_Materiales");

        builder.Property(e => e.MaterialId).HasColumnName("MaterialId");
        builder.Property(e => e.Codigo).HasColumnName("Codigo").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(300);
        builder.Property(e => e.UnidadMedida).HasColumnName("UnidadMedida").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Sku).HasColumnName("SKU").HasMaxLength(100);
        builder.Property(e => e.CategoriaId).HasColumnName("CategoriaId");
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2(7)");
        builder.Property(e => e.CreadoPor).HasColumnName("CreadoPor");
        builder.Property(e => e.ModificadoPor).HasColumnName("ModificadoPor");
        builder.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue(true);

        builder.HasIndex(e => e.Codigo)
            .HasDatabaseName("UX_Materiales_Codigo")
            .IsUnique();

        builder.HasIndex(e => e.Sku)
            .HasDatabaseName("UX_Materiales_SKU")
            .IsUnique()
            .HasFilter("[SKU] IS NOT NULL");

        builder.HasOne(e => e.Categoria)
            .WithMany(e => e.Materiales)
            .HasForeignKey(e => e.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Materiales_Categoria");
    }
}
