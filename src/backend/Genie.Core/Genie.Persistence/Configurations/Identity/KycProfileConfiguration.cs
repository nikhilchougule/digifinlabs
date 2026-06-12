using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Entities;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Identity
{
    public sealed class KycProfileConfiguration : IEntityTypeConfiguration<KycProfile>
    {
        public void Configure(EntityTypeBuilder<KycProfile> builder)
        {
            builder.ToTable("kyc_profiles", "identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.FaceMatchScore).HasPrecision(5, 4);
            builder.Property(x => x.LivenessVerified).HasDefaultValue(false);
            builder.Property(x => x.VerifiedAt);
            builder.Property(x => x.Provider).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ExpiryDate);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.AadhaarNumber, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("aadhaar_cipher").IsRequired();
                enc.Property(e => e.KeyVersion).HasColumnName("aadhaar_key_ver").HasDefaultValue(1);
            });

            builder.OwnsOne(x => x.PanNumber, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("pan_cipher").IsRequired();
                enc.Property(e => e.KeyVersion).HasColumnName("pan_key_ver").HasDefaultValue(1);
            });

            builder.Property(x => x.RawResponse)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.UserId);
        }
    }
}
