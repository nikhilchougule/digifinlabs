using Los.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class ApplicationProductConfiguration : IEntityTypeConfiguration<ApplicationProduct>
    {
        public void Configure(EntityTypeBuilder<ApplicationProduct> builder)
        {
            builder.ToTable("application_products", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.PurposeDescription).HasMaxLength(1000);
            builder.Property(x => x.TenureMonths);
            builder.Property(x => x.DisbursementMode).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.RepaymentFrequency).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.RequestedAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("requested_amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("requested_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.Parameters)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.ApplicationId).IsUnique();
        }
    }
}
