using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// Aggregated financial snapshot for a Personal user — the core of the Finance Dashboard UI.
    ///
    /// DESIGN RULES:
    ///   - TotalAssets, TotalLiabilities, and NetWorth are COMPUTED at refresh time. Never derive live.
    ///   - NetWorthHistory is append-only — historical snapshots must never be removed.
    ///     Enables the net worth trend chart in the UI.
    ///   - MonthlyCashflowSummary is regenerated on each refresh; replace the current month's
    ///     entry rather than appending duplicates.
    ///   - LastRefreshedAt drives the "stale data" indicator — warn in UI if older than 24 hours.
    /// </summary>
    public sealed class FinanceDashboard : BaseAuditableEntity
    {
        public required Guid UserId { get; init; }

        /// <summary>Ids of all AA-linked BankAccounts included in this snapshot (JSONB).</summary>
        public List<Guid> LinkedAccountIds { get; set; } = new();

        /// <summary>Sum of all positive asset values at last refresh, in paise.</summary>
        public Money TotalAssets { get; set; } = new Money(0L);

        /// <summary>Sum of all outstanding loan liabilities at last refresh, in paise.</summary>
        public Money TotalLiabilities { get; set; } = new Money(0L);

        /// <summary>TotalAssets − TotalLiabilities. Recomputed on every refresh.</summary>
        public Money NetWorth { get; set; } = new Money(0L);

        /// <summary>
        /// Monthly cashflow summary as JSONB array.
        /// Shape: [{ month: "YYYY-MM", creditsPaise: long, debitsPaise: long, savingsPaise: long }]
        /// </summary>
        public List<Dictionary<string, object>> MonthlyCashflowSummary { get; set; } = new();

        /// <summary>
        /// Time-series net worth snapshots — append-only for trend charting.
        /// Shape: [{ date: "YYYY-MM-DD", netWorthPaise: long }]
        /// NEVER remove historical entries.
        /// </summary>
        public List<Dictionary<string, object>> NetWorthHistory { get; set; } = new();

        /// <summary>
        /// UTC timestamp of last AA data refresh. Null until first refresh completes.
        /// Surface a "stale data" warning in UI if older than 24 hours.
        /// </summary>
        public DateTime? LastRefreshedAt { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (TotalAssets.AmountInPaise < 0)
                yield return "TotalAssets cannot be negative.";

            if (TotalLiabilities.AmountInPaise < 0)
                yield return "TotalLiabilities cannot be negative.";
        }

        public override string GetDisplayName() => $"Dashboard[User:{UserId}]:{Id}";
    }
}
