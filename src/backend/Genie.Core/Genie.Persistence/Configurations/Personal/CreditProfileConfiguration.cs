using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;
using System.Text.Json;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class CreditProfileConfiguration : IEntityTypeConfiguration<CreditProfile>
    {
        public void Configure(EntityTypeBuilder<CreditProfile> builder)
        {
            builder.ToTable("credit_profiles", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Bureau).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.ConsentId).IsRequired();
            builder.Property(x => x.ReportDate).IsRequired();
            builder.Property(x => x.NextRefreshDate);
            builder.Property(x => x.ActiveLoansCount).HasDefaultValue(0);
            builder.Property(x => x.DpdCount).HasDefaultValue(0);
            builder.Property(x => x.RawReportVaultId);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            // Bureau score is PII — stored encrypted (CICRA mandate)
            builder.OwnsOne(x => x.Score, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("score_cipher").IsRequired();
                enc.Property(e => e.KeyVersion).HasColumnName("score_key_ver").HasDefaultValue(1);
            });

            builder.Property(x => x.ImprovementTips)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Dictionary<string, object>>>(v, (JsonSerializerOptions?)null) ?? new());

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.Bureau });
            builder.HasIndex(x => x.ReportDate);
        }
    }
}
