using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("bank_accounts", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Ifsc).HasMaxLength(11).IsRequired();
            builder.Property(x => x.BankName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.AccountType).HasMaxLength(20).IsRequired();
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

            // Nullable balance snapshot
            builder.OwnsOne(x => x.BalanceSnapshot, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("balance_snapshot_paise");
                m.Property(p => p.CurrencyCode).HasColumnName("balance_snapshot_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ConsentId);
        }
    }
}
