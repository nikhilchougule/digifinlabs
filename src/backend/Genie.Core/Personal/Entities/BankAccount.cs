using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// A bank account linked via the RBI Account Aggregator (AA) framework.
    ///
    /// DESIGN RULES:
    ///   - AccountNumber is AES-256 encrypted PII. Display only the last 4 digits in UI.
    ///   - ConsentId is mandatory — every AA data pull must reference a valid ConsentLog entry.
    ///   - ConsentExpiry MUST be checked before every sync call.
    ///     Pull after expiry = RBI AA violation. Re-consent required.
    ///   - BalanceSnapshot is a point-in-time value — never treat it as real-time.
    /// </summary>
    public sealed class BankAccount : BaseEncryptedEntity
    {
        public required Guid UserId { get; init; }

        /// <summary>
        /// Full account number, AES-256 encrypted.
        /// Never log or cache the plaintext. UI shows masked form: XXXX XXXX XXXX 1234.
        /// </summary>
        public required EncryptedValue AccountNumber { get; init; }

        /// <summary>11-character RBI-format IFSC code.</summary>
        public required string Ifsc { get; set; }

        public required string BankName { get; set; }

        /// <summary>SAVINGS | CURRENT | OD | CC</summary>
        public required string AccountType { get; set; }

        /// <summary>
        /// RBI AA ConsentLog reference — mandatory.
        /// Maps to ConsentLog.Id in the SharedKernel.
        /// </summary>
        public required Guid ConsentId { get; init; }

        /// <summary>
        /// Expiry of the AA consent. Check before every sync.
        /// Expired consent = no data pull allowed until user re-consents.
        /// </summary>
        public required DateTime ConsentExpiry { get; set; }

        /// <summary>Last known closing balance in paise — snapshot at LastSyncedAt. Null until first sync.</summary>
        public Money? BalanceSnapshot { get; set; }

        public DateTime? LastSyncedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public List<BankTransaction> Transactions { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (string.IsNullOrWhiteSpace(Ifsc) || Ifsc.Length != 11)
                yield return "IFSC must be exactly 11 characters (RBI format).";

            if (string.IsNullOrWhiteSpace(BankName))
                yield return "BankName is required.";

            if (ConsentId == Guid.Empty)
                yield return "BankAccount must reference a ConsentLog entry (RBI AA mandate).";

            if (ConsentExpiry <= DateTime.UtcNow)
                yield return "Consent has expired. Re-consent required before any data sync.";
        }

        public override string GetDisplayName() => $"Bank[{BankName}|{AccountType}]:{Id}";
    }
}
