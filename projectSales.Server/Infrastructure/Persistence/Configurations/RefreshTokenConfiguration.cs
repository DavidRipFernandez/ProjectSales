using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_RefreshTokens");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("userId");
        builder.Property(e => e.SessionId).HasColumnName("sessionId");
        builder.Property(e => e.TokenHash)
            .HasColumnName("tokenHash")
            .HasColumnType("varbinary(32)")
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdAt")
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.ExpiresAt).HasColumnName("expiresAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.RevokedAt).HasColumnName("revokedAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.RotatedFromTokenId).HasColumnName("rotatedFromTokenId");
        builder.Property(e => e.ReplacedByTokenId).HasColumnName("replacedByTokenId");

        builder.HasIndex(e => e.TokenHash)
            .HasDatabaseName("UX_RefreshTokens_Hash")
            .IsUnique();

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_RefreshTokens_User");

        builder.HasIndex(e => e.SessionId)
            .HasDatabaseName("IX_RefreshTokens_Session");

        builder.HasOne(e => e.User)
            .WithMany(e => e.RefreshTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RefreshTokens_User");

        builder.HasOne(e => e.Session)
            .WithMany(e => e.RefreshTokens)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RefreshTokens_Session");

        builder.HasOne(e => e.RotatedFromToken)
            .WithMany(e => e.RotatedTokens)
            .HasForeignKey(e => e.RotatedFromTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RefreshTokens_RotatedFrom");

        builder.HasOne(e => e.ReplacedByToken)
            .WithMany(e => e.ReplacedTokens)
            .HasForeignKey(e => e.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RefreshTokens_ReplacedBy");
    }
}
