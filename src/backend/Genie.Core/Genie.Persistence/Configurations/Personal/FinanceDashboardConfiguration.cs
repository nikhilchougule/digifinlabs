using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class FinanceDashboardConfiguration : IEntityTypeConfiguration<FinanceDashboard>
    {
        public void Configure(EntityTypeBuilder<FinanceDashboard> builder)
        {
            builder.ToTable("finance_dashboards", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId).IsUnique(); // 1:1 per user
            builder.Property(x => x.LastRefreshedAt);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.TotalAssets, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("total_assets_paise").HasDefaultValue(0L);
                m.Property(p => p.CurrencyCode).HasColumnName("total_assets_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.TotalLiabilities, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("total_liabilities_paise").HasDefaultValue(0L);
                m.Property(p => p.CurrencyCode).HasColumnName("total_liabilities_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.NetWorth, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("net_worth_paise").HasDefaultValue(0L);
                m.Property(p => p.CurrencyCode).HasColumnName("net_worth_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.LinkedAccountIds)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.Property(x => x.MonthlyCashflowSummary)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Dictionary<string, object>>>(v, (JsonSerializerOptions?)null) ?? new());

            // Append-only history — never delete entries, only add
            builder.Property(x => x.NetWorthHistory)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Dictionary<string, object>>>(v, (JsonSerializerOptions?)null) ?? new());
        }
    }
}
