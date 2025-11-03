using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_Users");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(e => e.PasswordHash).HasColumnName("passwordHash").HasMaxLength(256).IsRequired();
        builder.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(e => e.UltimoLoginUtc).HasColumnName("ultimoLoginUtc").HasColumnType("datetime2(3)");
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

        builder.HasIndex(e => e.Username)
            .HasDatabaseName("UX_Users_username")
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("UX_Users_email")
            .IsUnique();
    }
}
