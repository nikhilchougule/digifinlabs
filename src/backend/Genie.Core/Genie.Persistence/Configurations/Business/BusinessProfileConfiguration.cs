using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Enums;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
    {
        public void Configure(EntityTypeBuilder<BusinessProfile> builder)
        {
            builder.ToTable("profiles", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.LegalName).HasMaxLength(300).IsRequired();
            builder.Property(x => x.TradeName).HasMaxLength(300);
            builder.Property(x => x.EntityType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.UdyamNumber).HasMaxLength(30);
            builder.Property(x => x.IncorporationDate);
            builder.Property(x => x.IndustryCode).HasMaxLength(10);
            builder.Property(x => x.AnnualTurnoverBand).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.KycProfileId);
            builder.Property(x => x.KycStatus).HasConversion<string>().HasMaxLength(30).HasDefaultValue(KycStatus.Pending);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // Encrypted PAN & GSTIN (regulatory data)
            builder.OwnsOne(x => x.Gstin, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("gstin_cipher");
                enc.Property(e => e.KeyVersion).HasColumnName("gstin_key_ver").HasDefaultValue(1);
            });

            builder.OwnsOne(x => x.Pan, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("pan_cipher");
                enc.Property(e => e.KeyVersion).HasColumnName("pan_key_ver").HasDefaultValue(1);
            });

            builder.OwnsOne(x => x.Address, addr =>
            {
                addr.Property(a => a.Line1).HasColumnName("address_line1").HasMaxLength(300);
                addr.Property(a => a.Line2).HasColumnName("address_line2").HasMaxLength(300);
                addr.Property(a => a.City).HasColumnName("address_city").HasMaxLength(100);
                addr.Property(a => a.State).HasColumnName("address_state").HasMaxLength(100);
                addr.Property(a => a.Pincode).HasColumnName("address_pincode").HasMaxLength(10);
                addr.Property(a => a.Country).HasColumnName("address_country").HasMaxLength(100).HasDefaultValue("India");
                addr.OwnsOne(a => a.Coordinates, geo =>
                {
                    geo.Property(g => g.Latitude).HasColumnName("address_lat");
                    geo.Property(g => g.Longitude).HasColumnName("address_lng");
                });
            });

            builder.HasIndex(x => x.UserId);
        }
    }
}
