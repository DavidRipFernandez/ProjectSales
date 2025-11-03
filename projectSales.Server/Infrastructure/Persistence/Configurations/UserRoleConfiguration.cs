using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "dbo");

        builder.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK_UserRoles");

        builder.Property(e => e.UserId).HasColumnName("userId");
        builder.Property(e => e.RoleId).HasColumnName("roleId");
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

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserRoles)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_UserRoles_Users");

        builder.HasOne(e => e.Role)
            .WithMany(e => e.UserRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_UserRoles_Roles");
    }
}
