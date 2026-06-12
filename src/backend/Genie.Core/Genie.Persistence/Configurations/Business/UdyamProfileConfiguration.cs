using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class UdyamProfileConfiguration : IEntityTypeConfiguration<UdyamProfile>
    {
        public void Configure(EntityTypeBuilder<UdyamProfile> builder)
        {
            builder.ToTable("udyam_profiles", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessProfileId).IsRequired();
            builder.HasIndex(x => x.BusinessProfileId).IsUnique();
            builder.Property(x => x.UdyamNumber).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => x.UdyamNumber).IsUnique();
            builder.Property(x => x.RegistrationDate).IsRequired();
            builder.Property(x => x.Classification).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.InvestmentInMachinery, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("investment_machinery_paise").HasDefaultValue(0L);
                m.Property(p => p.CurrencyCode).HasColumnName("investment_machinery_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.AnnualTurnover, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("annual_turnover_paise").HasDefaultValue(0L);
                m.Property(p => p.CurrencyCode).HasColumnName("annual_turnover_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
        }
    }
}
