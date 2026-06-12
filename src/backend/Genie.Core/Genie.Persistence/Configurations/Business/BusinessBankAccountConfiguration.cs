using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genie.Persistence.Configurations.Business
{
    public sealed class BusinessBankAccountConfiguration : IEntityTypeConfiguration<BusinessBankAccount>
    {
        public void Configure(EntityTypeBuilder<BusinessBankAccount> builder)
        {
            builder.ToTable("bank_accounts", "business");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.BusinessProfileId).IsRequired();
            builder.Property(x => x.Ifsc).HasMaxLength(11).IsRequired();
            builder.Property(x => x.BankName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.AccountType).HasMaxLength(30).IsRequired();
            builder.Property(x => x.ConsentId).IsRequired();
            builder.Property(x => x.ConsentExpiry).IsRequired();
            builder.Property(x => x.LastSyncedAt);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.EncryptionKeyVersion).HasDefaultValue(1);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.AccountNumber, enc =>
            {
                enc.Property(e => e.CipherText).HasColumnName("account_number_cipher").IsRequired();
                enc.Property(e => e.KeyVersion).HasColumnName("account_number_key_ver").HasDefaultValue(1);
            });

            builder.OwnsOne(x => x.AnnualTurnover, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("annual_turnover_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("annual_turnover_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.OwnsOne(x => x.AverageMonthlyBalance, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("avg_monthly_balance_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("avg_monthly_balance_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasIndex(x => x.BusinessProfileId);
            builder.HasIndex(x => x.ConsentId);
        }
    }
}
