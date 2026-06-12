using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;

namespace Genie.Persistence.Configurations.Identity
{
    public sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
    {
        public void Configure(EntityTypeBuilder<Credential> builder)
        {
            builder.ToTable("credentials", "identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Provider).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ProviderKey).HasMaxLength(500).IsRequired();
            builder.Property(x => x.PasswordHash).HasMaxLength(500);
            builder.Property(x => x.IsVerified).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // One user can have multiple credentials (e.g. Email + Google + MobileOtp)
            builder.HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();
            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
