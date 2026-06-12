using Los.Servicing.Entities;
using Los.Servicing.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class RepaymentConfiguration : IEntityTypeConfiguration<Repayment>
    {
        public void Configure(EntityTypeBuilder<Repayment> builder)
        {
            builder.ToTable("repayments", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ScheduleId).IsRequired();
            builder.Property(x => x.InstallmentNumber).IsRequired();
            builder.Property(x => x.DueDate).IsRequired();
            builder.Property(x => x.PaidAt);
            builder.Property(x => x.DpdDays).HasDefaultValue(0);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(RepaymentStatus.Scheduled);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.DueAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("due_amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("due_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.PaidAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("paid_amount_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("paid_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasIndex(x => x.ScheduleId);
            builder.HasIndex(x => new { x.ScheduleId, x.InstallmentNumber }).IsUnique();
            builder.HasIndex(x => x.DueDate);
        }
    }
}
