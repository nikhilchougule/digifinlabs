using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;

namespace Genie.Persistence.Configurations.Identity
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens", "identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.TokenHash).HasMaxLength(500).IsRequired();
            builder.Property(x => x.DeviceFingerprint).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.IsRevoked).HasDefaultValue(false);
            builder.Property(x => x.RevokedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);
            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsActive);

            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
