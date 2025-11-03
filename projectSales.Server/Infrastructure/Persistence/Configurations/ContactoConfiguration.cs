using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class ContactoConfiguration : IEntityTypeConfiguration<Contacto>
{
    public void Configure(EntityTypeBuilder<Contacto> builder)
    {
        builder.ToTable("Contactos", "dbo");

        builder.HasKey(e => e.ContactoId).HasName("PK_Contactos");

        builder.Property(e => e.ContactoId).HasColumnName("ContactoId");
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Correo).HasColumnName("Correo").HasMaxLength(100);
        builder.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(100);
        builder.Property(e => e.ProveedorCifId).HasColumnName("ProveedorCifId").HasMaxLength(50).IsRequired();
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("FechaCreacion")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2(7)");
        builder.Property(e => e.CreadoPor).HasColumnName("CreadoPor");
        builder.Property(e => e.ModificadoPor).HasColumnName("ModificadoPor");
        builder.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue(true);

        builder.HasIndex(e => e.ProveedorCifId)
            .HasDatabaseName("IX_Contactos_Proveedor");

        builder.HasOne(e => e.Proveedor)
            .WithMany(e => e.Contactos)
            .HasForeignKey(e => e.ProveedorCifId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Contactos_Proveedor");
    }
}
