using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class PromoterConfiguration : IEntityTypeConfiguration<Promoter>
    {
        public void Configure(EntityTypeBuilder<Promoter> builder)
        {
            builder.ToTable("promoters", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessProfileId).IsRequired();
            builder.Property(x => x.PersonalProfileId);
            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.Din).HasMaxLength(10);
            builder.Property(x => x.ShareholdingPercent).HasPrecision(5, 2);
            builder.Property(x => x.IsGuarantor).HasDefaultValue(false);
            builder.Property(x => x.DateOfAppointment);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // PAN is mandatory for KYC
            builder.OwnsOne(x => x.PanNumber, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("pan_cipher").IsRequired();
                enc.Property(e => e.KeyVersion).HasColumnName("pan_key_ver").HasDefaultValue(1);
            });

            builder.HasOne<BusinessProfile>()
                .WithMany()
                .HasForeignKey(x => x.BusinessProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BusinessProfileId);
        }
    }
}
