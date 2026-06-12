using Los.Servicing.Entities;
using Los.Servicing.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class DisbursementConfiguration : IEntityTypeConfiguration<Disbursement>
    {
        public void Configure(EntityTypeBuilder<Disbursement> builder)
        {
            builder.ToTable("disbursements", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.TrancheNumber).HasDefaultValue(1).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(DisbursementStatus.Pending);
            builder.Property(x => x.DisbursedAt);
            builder.Property(x => x.BankRefNumber).HasMaxLength(100);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.Amount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => new { x.ApplicationId, x.TrancheNumber }).IsUnique();
        }
    }
}
