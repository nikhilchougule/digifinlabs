using Los.Underwriting.Entities;
using Los.Underwriting.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class IncomeAssessmentConfiguration : IEntityTypeConfiguration<IncomeAssessment>
    {
        public void Configure(EntityTypeBuilder<IncomeAssessment> builder)
        {
            builder.ToTable("income_assessments", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.AssessmentType).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.EmployerStabilityScore).HasPrecision(5, 2);
            builder.Property(x => x.VintageWithEmployerMonths);
            builder.Property(x => x.IncomeVarianceFlag).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.GrossMonthlySalary, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("gross_monthly_salary_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("gross_monthly_salary_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.NetTakeHome, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("net_take_home_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("net_take_home_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.AvgMonthlyTurnover, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("avg_monthly_turnover_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("avg_monthly_turnover_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.GstDeclaredTurnover, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("gst_declared_turnover_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("gst_declared_turnover_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.ItrDeclaredIncome, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("itr_declared_income_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("itr_declared_income_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.BankingDerivedIncome, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("banking_derived_income_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("banking_derived_income_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.AdjustedIncome, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("adjusted_income_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("adjusted_income_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.IncomeSourceBreakdown)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.ApplicationId);
        }
    }
}
