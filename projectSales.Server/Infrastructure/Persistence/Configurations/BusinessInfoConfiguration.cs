using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class BusinessInfoConfiguration : IEntityTypeConfiguration<BusinessInfo>
{
    public void Configure(EntityTypeBuilder<BusinessInfo> builder)
    {
        builder.ToTable("BusinessInfo", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_BusinessInfo");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.EmpresaNombre).HasColumnName("empresaNombre").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Nit).HasColumnName("nit").HasMaxLength(50);
        builder.Property(e => e.ContactoNombre).HasColumnName("contactoNombre").HasMaxLength(150);
        builder.Property(e => e.ContactoTelefono).HasColumnName("contactoTelefono").HasMaxLength(50);
        builder.Property(e => e.ContactoEmail).HasColumnName("contactoEmail").HasMaxLength(256);
        builder.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(300);
        builder.Property(e => e.Ciudad).HasColumnName("ciudad").HasMaxLength(100);
        builder.Property(e => e.Pais).HasColumnName("pais").HasMaxLength(100);
        builder.Property(e => e.Web).HasColumnName("web").HasMaxLength(200);
        builder.Property(e => e.Facebook).HasColumnName("facebook").HasMaxLength(200);
        builder.Property(e => e.Instagram).HasColumnName("instagram").HasMaxLength(200);
        builder.Property(e => e.Tiktok).HasColumnName("tiktok").HasMaxLength(200);
        builder.Property(e => e.IsPrimary).HasColumnName("isPrimary").HasDefaultValue(true);
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

        builder.HasIndex(e => e.IsPrimary)
            .HasDatabaseName("UX_BusinessInfo_Primary")
            .IsUnique()
            .HasFilter("[isPrimary] = 1");
    }
}
