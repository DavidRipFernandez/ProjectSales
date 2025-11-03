using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_Roles");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(250);
        builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(e => e.CreadoPor).HasColumnName("creadoPor").HasMaxLength(100);
        builder.Property(e => e.FechaCreacion)
            .HasColumnName("fechaCreacion")
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.ModificadoPor).HasColumnName("modificadoPor").HasMaxLength(100);
        builder.Property(e => e.FechaModificacion).HasColumnName("fechaModificacion").HasColumnType("datetime2(3)");
        builder.Property(e => e.RowVersion)
            .HasColumnName("rowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(e => e.Nombre)
            .HasDatabaseName("UX_Roles_nombre")
            .IsUnique();
    }
}
