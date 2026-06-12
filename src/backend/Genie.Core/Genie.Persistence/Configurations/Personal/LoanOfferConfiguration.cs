using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;
using Personal.Enums;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class LoanOfferConfiguration : IEntityTypeConfiguration<LoanOffer>
    {
        public void Configure(EntityTypeBuilder<LoanOffer> builder)
        {
            builder.ToTable("loan_offers", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.LenderId).IsRequired();
            builder.Property(x => x.ProductType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.InterestRate).HasPrecision(8, 6).IsRequired(); // e.g. 0.115000
            builder.Property(x => x.TenureMonths).IsRequired();
            builder.Property(x => x.EligibilityScore).HasPrecision(5, 2).IsRequired();
            builder.Property(x => x.OfferExpiry).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(LoanOfferStatus.Shown);
            builder.Property(x => x.UtmRef).HasMaxLength(200);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.OfferedAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("offered_amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("offered_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.Status });
            builder.HasIndex(x => x.OfferExpiry);
        }
    }
}
