using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;
using SharedKernel.Enums;

namespace Genie.Persistence.Configurations.Shared
{
    public sealed class DocumentVaultConfiguration : IEntityTypeConfiguration<DocumentVault>
    {
        public void Configure(EntityTypeBuilder<DocumentVault> builder)
        {
            builder.ToTable("document_vaults", "shared");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.OwnerId).IsRequired();
            builder.Property(x => x.OwnerType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.DocType).HasConversion<string>().HasMaxLength(100).IsRequired();
            // StorageKey is the object storage key — never a URL
            builder.Property(x => x.StorageKey).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.VerificationStatus)
                .HasConversion<string>().HasMaxLength(50)
                .HasDefaultValue(DocumentVerificationStatus.Pending);
            builder.Property(x => x.VerifiedByMethod).HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.VerifiedByUserId);
            builder.Property(x => x.ExpiryDate);
            builder.Property(x => x.UploadedAt).IsRequired();
            builder.Property(x => x.VerifiedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);
            builder.Ignore(x => x.IsVerified);

            builder.HasMany(x => x.SharedWith)
                .WithOne(x => x.Vault)
                .HasForeignKey(x => x.VaultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.OwnerId, x.OwnerType });
            builder.HasIndex(x => x.DocType);
        }
    }
}
