using Los.Underwriting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class UnderwritingAssessmentConfiguration : IEntityTypeConfiguration<UnderwritingAssessment>
    {
        public void Configure(EntityTypeBuilder<UnderwritingAssessment> builder)
        {
            builder.ToTable("underwriting_assessments", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.UnderwriterId).IsRequired();
            builder.Property(x => x.IncomeAssessmentId);
            builder.Property(x => x.BankingAnalysisId);
            builder.Property(x => x.GstAnalysisId);
            builder.Property(x => x.BureauCreditProfileId);
            builder.Property(x => x.Dscr).HasPrecision(8, 4);
            builder.Property(x => x.Ltv).HasPrecision(5, 4);
            builder.Property(x => x.Foir).HasPrecision(5, 4);
            builder.Property(x => x.TurnoverMultiple).HasPrecision(8, 4);
            builder.Property(x => x.LeverageRatio).HasPrecision(8, 4);
            builder.Property(x => x.CurrentRatio).HasPrecision(8, 4);
            builder.Property(x => x.EmiToIncomeRatio).HasPrecision(5, 4);
            builder.Property(x => x.RecommendedTenure);
            builder.Property(x => x.RecommendedRate).HasPrecision(8, 6);
            builder.Property(x => x.CollateralRequirement).HasDefaultValue(false);
            builder.Property(x => x.UnderwriterNotes).HasMaxLength(5000);
            builder.Property(x => x.SubmittedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.RecommendedAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("recommended_amount_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("recommended_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.CollateralDetails)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.ExceptionFlags)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => x.UnderwriterId);
        }
    }
}
