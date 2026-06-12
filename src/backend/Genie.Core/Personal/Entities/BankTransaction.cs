using Personal.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// A single bank transaction pulled via the RBI Account Aggregator framework.
    ///
    /// DESIGN RULES:
    ///   - TxnId is the idempotency key. AA resends past transactions on every re-sync.
    ///     Dedup on (AccountId, TxnId) before insert — no duplicates allowed.
    ///   - Amount uses the Money value object (paise-based). Never use decimal for storage.
    ///   - Category is set asynchronously by the ML categorisation pipeline — may be null
    ///     immediately after ingestion.
    /// </summary>
    public sealed class BankTransaction : BaseEntity
    {
        public required Guid AccountId { get; init; }

        /// <summary>
        /// AA-provided unique transaction reference per account.
        /// Idempotency key — dedup on (AccountId, TxnId) before every insert.
        /// </summary>
        public required string TxnId { get; init; }

        /// <summary>Transaction amount in paise. Always positive — Type indicates direction.</summary>
        public required Money Amount { get; init; }

        public required TransactionType Type { get; init; }

        /// <summary>
        /// Auto-tagged by the ML categorisation pipeline.
        /// Null until the async categorisation job processes this transaction.
        /// </summary>
        public TransactionCategory? Category { get; set; }

        /// <summary>Merchant name extracted from transaction narration. May be null.</summary>
        public string? Merchant { get; set; }

        /// <summary>Raw narration / description from the bank statement.</summary>
        public string? Description { get; set; }

        public required DateTime TxnDate { get; init; }

        // Navigation
        public BankAccount? Account { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (AccountId == Guid.Empty)
                yield return "AccountId must not be empty.";

            if (string.IsNullOrWhiteSpace(TxnId))
                yield return "TxnId (idempotency key) must not be empty.";

            if (Amount.AmountInPaise <= 0)
                yield return "Transaction amount must be greater than zero.";

            if (TxnDate > DateTime.UtcNow)
                yield return "TxnDate cannot be in the future.";
        }
    }
}
