using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// AA-linked business bank account — the primary banking signal for MSME underwriting.
    ///
    /// DESIGN RULES:
    ///   - AccountNumber is AES-256 encrypted PII.
    ///   - ConsentId is mandatory — AA pull without a valid ConsentLog entry is a regulatory violation.
    ///   - AnnualTurnover is the sum of all credits in the last 12 months from AA data.
    ///     This is the single most powerful underwriting signal for MSMEs without audited accounts.
    ///   - Multiple accounts may be linked per BusinessProfile (current + OD + CC).
    /// </summary>
    public sealed class BusinessBankAccount : BaseEncryptedEntity
    {
        public required Guid BusinessProfileId { get; init; }

        /// <summary>Bank account number, AES-256 encrypted.</summary>
        public required EncryptedValue AccountNumber { get; init; }

        /// <summary>11-character IFSC code.</summary>
        public required string Ifsc { get; init; }

        public required string BankName { get; set; }

        /// <summary>SAVINGS | CURRENT | OD | CC</summary>
        public required string AccountType { get; init; }

        /// <summary>
        /// Mandatory AA consent reference. FK → ConsentLog.Id (ConsentType.BankData).
        /// No bank data access without a valid consent record — RBI AA framework.
        /// </summary>
        public required Guid ConsentId { get; init; }

        public required DateTime ConsentExpiry { get; init; }

        /// <summary>
        /// Total credits in the trailing 12 months — primary MSME income proxy.
        /// Recomputed on each AA sync. Null until first sync.
        /// </summary>
        public Money? AnnualTurnover { get; set; }

        /// <summary>Average monthly balance from AA data. Null until first sync.</summary>
        public Money? AverageMonthlyBalance { get; set; }

        public DateTime? LastSyncedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessProfileId == Guid.Empty)
                yield return "BusinessProfileId must not be empty.";

            if (string.IsNullOrWhiteSpace(Ifsc) || Ifsc.Length != 11)
                yield return "IFSC must be exactly 11 characters.";

            if (ConsentId == Guid.Empty)
                yield return "ConsentId must reference a valid ConsentLog entry (RBI AA mandate).";

            if (ConsentExpiry <= DateTime.UtcNow)
                yield return "Consent has expired — re-consent required before syncing bank data.";
        }

        public override string GetDisplayName() => $"BizBank[{BankName}|{AccountType}]:{Id}";
    }
}
