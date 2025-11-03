using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_RolePermissions");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.RoleId).HasColumnName("roleId");
        builder.Property(e => e.ModuleId).HasColumnName("moduleId");
        builder.Property(e => e.ActionId).HasColumnName("actionId");
        builder.Property(e => e.Allowed).HasColumnName("allowed");
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

        builder.HasIndex(e => new { e.RoleId, e.ModuleId, e.ActionId })
            .HasDatabaseName("UX_RolePerms_UQ")
            .IsUnique();

        builder.HasOne(e => e.Role)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RolePermissions_Roles");

        builder.HasOne(e => e.Module)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RolePermissions_Modules");

        builder.HasOne(e => e.Action)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.ActionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RolePermissions_Actions");
    }
}
