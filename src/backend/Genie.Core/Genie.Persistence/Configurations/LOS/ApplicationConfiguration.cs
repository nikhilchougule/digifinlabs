using Los.Application.Entities;
using Los.Application.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

// Alias prevents ambiguity between class Application and namespace Los.Application
using LosApplication = Los.Application.Entities.Application;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class ApplicationConfiguration : IEntityTypeConfiguration<LosApplication>
    {
        public void Configure(EntityTypeBuilder<LosApplication> builder)
        {
            builder.ToTable("applications", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationNumber).HasMaxLength(30).IsRequired();
            builder.HasIndex(x => x.ApplicationNumber).IsUnique();
            builder.Property(x => x.ApplicationType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ApplicantType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.ApplicantId).IsRequired();
            builder.Property(x => x.CurrentStage).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.AssignedSalesId);
            builder.Property(x => x.AssignedCreditId);
            builder.Property(x => x.AssignedRiskId);
            builder.Property(x => x.LenderProductId);
            builder.Property(x => x.TenureMonths);
            builder.Property(x => x.InterestRateOffered).HasPrecision(8, 6);
            builder.Property(x => x.TatStartAt);
            builder.Property(x => x.TatEndAt);
            builder.Property(x => x.TatSlaHours);
            builder.Property(x => x.TatBreachFlag).HasDefaultValue(false);
            builder.Property(x => x.AiRiskFlag).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.SubmittedAt);
            builder.Property(x => x.ClosedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.LoanAmountRequested, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("loan_amount_requested_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("loan_amount_requested_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.LoanAmountSanctioned, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("loan_amount_sanctioned_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("loan_amount_sanctioned_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.ApplicantSnapshot)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.Remarks)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasOne(x => x.Product)
                .WithOne()
                .HasForeignKey<ApplicationProduct>(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Documents)
                .WithOne()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.StageHistory)
                .WithOne()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ApplicantId);
            builder.HasIndex(x => x.CurrentStage);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
