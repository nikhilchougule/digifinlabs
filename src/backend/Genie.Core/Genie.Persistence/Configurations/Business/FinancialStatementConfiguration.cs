using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class FinancialStatementConfiguration : IEntityTypeConfiguration<FinancialStatement>
    {
        public void Configure(EntityTypeBuilder<FinancialStatement> builder)
        {
            builder.ToTable("financial_statements", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessProfileId).IsRequired();
            builder.Property(x => x.FiscalYear).HasMaxLength(9).IsRequired(); // e.g. "2023-24"
            builder.Property(x => x.StatementType).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.IsAudited).HasDefaultValue(false);
            builder.Property(x => x.AuditorName).HasMaxLength(200);
            builder.Property(x => x.VaultId).IsRequired(); // Ref to document_vaults
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.TotalRevenue, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("total_revenue_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("total_revenue_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.NetProfit, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("net_profit_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("net_profit_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.TotalAssets, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("total_assets_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("total_assets_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.TotalLiabilities, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("total_liabilities_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("total_liabilities_currency").HasMaxLength(3).HasDefaultValue("INR");
            });
            builder.OwnsOne(x => x.Ebitda, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("ebitda_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("ebitda_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.Property(x => x.ExtractedData)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.BusinessProfileId);
            builder.HasIndex(x => new { x.BusinessProfileId, x.FiscalYear, x.StatementType }).IsUnique();
        }
    }
}
