using Los.Application.Entities;
using Los.Application.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class TatTrackerConfiguration : IEntityTypeConfiguration<TatTracker>
    {
        public void Configure(EntityTypeBuilder<TatTracker> builder)
        {
            builder.ToTable("tat_trackers", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.HasIndex(x => x.ApplicationId).IsUnique();
            builder.Property(x => x.ProductSlaHours).IsRequired();
            builder.Property(x => x.TotalElapsedHours).HasPrecision(10, 2);
            builder.Property(x => x.SlaRemainingHours).HasPrecision(10, 2);
            builder.Property(x => x.BottleneckStage).HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.TatStatus).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.PredictedCompletionAt);
            builder.Property(x => x.EscalationTriggered).HasDefaultValue(false);
            builder.Property(x => x.EscalatedToId);
            builder.Property(x => x.LastUpdatedAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // StageTiming list stored as JSONB array
            builder.Property(x => x.StageTimings)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<StageTiming>>(v, (JsonSerializerOptions?)null) ?? new());
        }
    }
}
