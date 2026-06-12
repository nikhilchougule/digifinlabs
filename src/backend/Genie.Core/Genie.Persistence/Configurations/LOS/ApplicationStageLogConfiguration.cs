using Los.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    /// <summary>
    /// ApplicationStageLog is IAppendOnly — the AppendOnlyInterceptor blocks all UPDATEs/DELETEs.
    /// No UpdatedAt/DeletedAt columns are needed here since this is an immutable audit trail.
    /// </summary>
    public sealed class ApplicationStageLogConfiguration : IEntityTypeConfiguration<ApplicationStageLog>
    {
        public void Configure(EntityTypeBuilder<ApplicationStageLog> builder)
        {
            builder.ToTable("application_stage_logs", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.FromStage).HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.ToStage).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.TransitionedById).IsRequired();
            builder.Property(x => x.TransitionedAt).IsRequired();
            builder.Property(x => x.TimeSpentHours).HasPrecision(8, 2);
            builder.Property(x => x.ReasonCode).HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.SlaBreached).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => x.TransitionedAt);
        }
    }
}
