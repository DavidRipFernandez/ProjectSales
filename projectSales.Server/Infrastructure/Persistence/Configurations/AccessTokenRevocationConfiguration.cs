using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class AccessTokenRevocationConfiguration : IEntityTypeConfiguration<AccessTokenRevocation>
{
    public void Configure(EntityTypeBuilder<AccessTokenRevocation> builder)
    {
        builder.ToTable("AccessTokenRevocations", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_AccessTokenRevocations");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Jti)
            .HasColumnName("jti")
            .HasColumnType("char(36)")
            .HasMaxLength(36)
            .IsRequired()
            .IsFixedLength();
        builder.Property(e => e.SessionId).HasColumnName("sessionId");
        builder.Property(e => e.RevokedAt)
            .HasColumnName("revokedAt")
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.ExpiresAt).HasColumnName("expiresAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(200);

        builder.HasIndex(e => e.Jti)
            .HasDatabaseName("UX_AccessRevocations_Jti")
            .IsUnique();

        builder.HasOne(e => e.Session)
            .WithMany(e => e.AccessTokenRevocations)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_AccessRevocations_Session");
    }
}
