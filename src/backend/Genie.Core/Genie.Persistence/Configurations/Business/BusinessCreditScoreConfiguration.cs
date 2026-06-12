using Business.Entities;
using Business.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class BusinessCreditScoreConfiguration : IEntityTypeConfiguration<BusinessCreditScore>
    {
        public void Configure(EntityTypeBuilder<BusinessCreditScore> builder)
        {
            builder.ToTable("credit_scores", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessId).IsRequired();
            builder.Property(x => x.Score).HasPrecision(7, 2).IsRequired(); // 0–1000
            builder.Property(x => x.RiskBand).HasConversion<string>().HasMaxLength(20).IsRequired();
            // BaseScorecardResult properties
            builder.Property(x => x.TotalScore).HasPrecision(5, 2).IsRequired();  // 0–100
            builder.Property(x => x.ScorecardVersion).HasMaxLength(20).IsRequired();
            builder.Property(x => x.ComputedAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // ScoreComponents stored as JSONB (sealed record with init props)
            builder.Property(x => x.Components)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<ScoreComponents>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.ParameterScores)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Dictionary<string, object>>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.Explainability)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.BusinessId);
            builder.HasIndex(x => x.ComputedAt);
        }
    }
}
