using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_Modules");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("isActive").HasDefaultValue(true);
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

        builder.HasIndex(e => e.Key)
            .HasDatabaseName("UX_Modules_key")
            .IsUnique();
    }
}
