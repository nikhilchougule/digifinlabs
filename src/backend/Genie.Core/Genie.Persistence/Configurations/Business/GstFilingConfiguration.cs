using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Business.Enums;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class GstFilingConfiguration : IEntityTypeConfiguration<GstFiling>
    {
        public void Configure(EntityTypeBuilder<GstFiling> builder)
        {
            builder.ToTable("gst_filings", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.GstProfileId).IsRequired();
            builder.Property(x => x.ReturnType).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.Period).HasMaxLength(10).IsRequired(); // e.g. "2024-03"
            builder.Property(x => x.FiledOn);
            builder.Property(x => x.FilingStatus).HasConversion<string>().HasMaxLength(20).HasDefaultValue(GstFilingStatus.Pending);
            builder.Property(x => x.DelayDays).HasDefaultValue(0);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // Nullable amounts (not all filings have them at creation time)
            builder.OwnsOne(x => x.TaxableTurnover, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("taxable_turnover_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("taxable_turnover_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.TaxPaid, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("tax_paid_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("tax_paid_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasIndex(x => x.GstProfileId);
            builder.HasIndex(x => new { x.GstProfileId, x.ReturnType, x.Period }).IsUnique();
        }
    }
}
