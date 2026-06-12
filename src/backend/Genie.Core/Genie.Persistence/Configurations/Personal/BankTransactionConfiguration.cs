using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal.Entities;

namespace Genie.Persistence.Configurations.Personal
{
    public sealed class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
    {
        public void Configure(EntityTypeBuilder<BankTransaction> builder)
        {
            builder.ToTable("bank_transactions", "personal");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.TxnId).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.Merchant).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.TxnDate).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Ignore(x => x.IsDeleted);

            builder.OwnsOne(x => x.Amount, m =>
            {
                m.Property(p => p.AmountInPaise).HasColumnName("amount_paise").IsRequired();
                m.Property(p => p.CurrencyCode).HasColumnName("amount_currency").HasMaxLength(3).HasDefaultValue("INR");
            });

            // Dedup constraint: one TxnId per account (AA idempotency)
            builder.HasIndex(x => new { x.AccountId, x.TxnId }).IsUnique();
            builder.HasIndex(x => x.TxnDate);
            builder.HasIndex(x => x.Category);
        }
    }
}
