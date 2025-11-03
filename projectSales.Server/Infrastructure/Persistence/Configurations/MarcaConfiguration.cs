using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> builder)
    {
        builder.ToTable("Marcas", "dbo");

        builder.HasKey(e => e.MarcaId).HasName("PK_Marcas");

        builder.Property(e => e.MarcaId).HasColumnName("MarcaId");
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(200);
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2(7)");
        builder.Property(e => e.CreadoPor).HasColumnName("CreadoPor");
        builder.Property(e => e.ModificadoPor).HasColumnName("ModificadoPor");
        builder.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue(true);

        builder.HasIndex(e => e.Nombre)
            .HasDatabaseName("UX_Marcas_Nombre")
            .IsUnique();
    }
}
