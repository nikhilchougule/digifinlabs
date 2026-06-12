using Los.Underwriting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class BankingAnalysisConfiguration : IEntityTypeConfiguration<BankingAnalysis>
    {
        public void Configure(EntityTypeBuilder<BankingAnalysis> builder)
        {
            builder.ToTable("banking_analyses", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            // SubjectId + SubjectType from BaseAnalysis (polymorphic — no DB FK constraint)
            builder.Property(x => x.SubjectId).IsRequired();
            builder.Property(x => x.SubjectType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.RequestId).HasMaxLength(200);
            builder.Property(x => x.BounceCount12m).HasDefaultValue(0);
            builder.Property(x => x.InwardChequeReturnCount).HasDefaultValue(0);
            builder.Property(x => x.OutwardChequeReturnCount).HasDefaultValue(0);
            builder.Property(x => x.CashDepositPercentage).HasPrecision(5, 4);
            builder.Property(x => x.SalaryCreditRegularity).HasPrecision(5, 4);
            builder.Property(x => x.EmiTrackRecord).HasPrecision(5, 4);
            builder.Property(x => x.BankingScore).HasPrecision(5, 2);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.AvgMonthlyCredits, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("avg_monthly_credits_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("avg_monthly_credits_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.AvgMonthlyDebits, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("avg_monthly_debits_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("avg_monthly_debits_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.AvgEodBalance, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("avg_eod_balance_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("avg_eod_balance_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.MinEodBalance, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("min_eod_balance_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("min_eod_balance_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.EmiObligationsDetected)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => new { x.SubjectId, x.SubjectType });
        }
    }
}
