using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class ModuleActionConfiguration : IEntityTypeConfiguration<ModuleAction>
{
    public void Configure(EntityTypeBuilder<ModuleAction> builder)
    {
        builder.ToTable("Actions", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_Actions");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ModuleId).HasColumnName("moduleId");
        builder.Property(e => e.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(e => e.SortOrder)
            .HasColumnName("sortOrder")
            .HasDefaultValue(0);
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

        builder.HasIndex(e => new { e.ModuleId, e.Key })
            .HasDatabaseName("UX_Actions_Module_Key")
            .IsUnique();

        builder.HasIndex(e => new { e.ModuleId, e.SortOrder })
            .HasDatabaseName("UX_Actions_Module_Sort")
            .IsUnique();

        builder.HasOne(e => e.Module)
            .WithMany(e => e.Actions)
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Actions_Modules");
    }
}
