using Los.Application.Enums;
using Los.Lending.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.ValueObjects;
using System.Text.Json;

namespace Genie.Persistence.Configurations.LOS
{
    public sealed class LenderProductConfiguration : IEntityTypeConfiguration<LenderProduct>
    {
        public void Configure(EntityTypeBuilder<LenderProduct> builder)
        {
            builder.ToTable("lender_products", "los");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.LenderId).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.MinRate).HasPrecision(8, 6).IsRequired();
            builder.Property(x => x.MaxRate).HasPrecision(8, 6).IsRequired();
            builder.Property(x => x.MinTenureMonths).IsRequired();
            builder.Property(x => x.MaxTenureMonths).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.MinAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("min_amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("min_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.MaxAmount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("max_amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("max_amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            // ApplicableTypes stored as JSON array of strings
            builder.Property(x => x.ApplicableTypes)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v.Select(t => t.ToString()), (JsonSerializerOptions?)null),
                    v => (JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new())
                             .Select(Enum.Parse<ApplicationType>).ToList());

            // RuleCondition list (SharedKernel value object)
            builder.Property(x => x.EligibilityCriteria)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<RuleCondition>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.LenderId);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
