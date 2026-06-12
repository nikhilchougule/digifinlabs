using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;

namespace Genie.Persistence.Configurations.Identity
{
    public sealed class OtpChallengeConfiguration : IEntityTypeConfiguration<OtpChallenge>
    {
        public void Configure(EntityTypeBuilder<OtpChallenge> builder)
        {
            builder.ToTable("otp_challenges", "identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Target).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.CodeHash).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.IsUsed).HasDefaultValue(false);
            builder.Property(x => x.AttemptCount).HasDefaultValue(0);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);
            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsValid);

            builder.HasIndex(x => new { x.Target, x.Purpose });
            builder.HasIndex(x => x.ExpiresAt); // for cleanup jobs
        }
    }
}
