using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions", "dbo");

        builder.HasKey(e => e.Id).HasName("PK_UserSessions");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.UserId).HasColumnName("userId");
        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdAt")
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.LastSeenAt).HasColumnName("lastSeenAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.IpAddress).HasColumnName("ipAddress").HasMaxLength(50);
        builder.Property(e => e.UserAgent).HasColumnName("userAgent").HasMaxLength(200);
        builder.Property(e => e.DeviceName).HasColumnName("deviceName").HasMaxLength(100);
        builder.Property(e => e.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(e => e.ExpiresAt).HasColumnName("expiresAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.RevokedAt).HasColumnName("revokedAt").HasColumnType("datetime2(3)");
        builder.Property(e => e.RevokeReason).HasColumnName("revokeReason").HasMaxLength(200);

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserSessions_User");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserSessions_Active")
            .HasFilter("[revokedAt] IS NULL")
            .IncludeProperties(e => new { e.CreatedAt, e.LastSeenAt });

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserSessions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_UserSessions_Users");
    }
}
