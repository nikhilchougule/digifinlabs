using Los.Servicing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class RepaymentScheduleConfiguration : IEntityTypeConfiguration<RepaymentSchedule>
    {
        public void Configure(EntityTypeBuilder<RepaymentSchedule> builder)
        {
            builder.ToTable("repayment_schedules", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.HasIndex(x => x.ApplicationId).IsUnique(); // One schedule per application
            builder.Property(x => x.Frequency).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.TotalInstallments).IsRequired();
            builder.Property(x => x.GeneratedAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.HasMany(x => x.Repayments)
                .WithOne()
                .HasForeignKey(x => x.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
