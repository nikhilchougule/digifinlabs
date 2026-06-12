using Personal.Enums;
using SharedKernel.Common;
using SharedKernel.Entities;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// Bureau credit report result for a user — one record per bureau pull.
    /// Multiple records accumulate over time (re-pulls, different bureaus).
    ///
    /// DESIGN RULES:
    ///   - Score is AES-256 encrypted PII — regulated under CICRA.
    ///     Every decrypt must be logged in AuditLog with Action = ACCESS.
    ///   - ConsentId is mandatory — bureau pull without explicit user consent = CICRA violation.
    ///   - RawReportVaultId → DocumentVault: the full PDF is in the vault, not inline here.
    ///     Never store multi-MB blobs in relational rows.
    ///   - ImprovementTips are AI-generated — regenerated on each pull, not manually edited.
    /// </summary>
    public sealed class CreditProfile : BaseEncryptedEntity
    {
        public required Guid UserId { get; init; }

        public required BureauProvider Bureau { get; init; }

        /// <summary>
        /// Bureau credit score, AES-256 encrypted (CICRA-regulated PII).
        /// Decrypt only within a trusted service boundary.
        /// </summary>
        public required EncryptedValue Score { get; init; }

        /// <summary>
        /// Consent reference — bureau pull without this = CICRA violation.
        /// Maps to ConsentLog.Id (ConsentType.BureauPull) in the SharedKernel.
        /// </summary>
        public required Guid ConsentId { get; init; }

        public required DateTime ReportDate { get; init; }

        /// <summary>Scheduled date for the next soft refresh. Null until set by the refresh scheduler.</summary>
        public DateTime? NextRefreshDate { get; set; }

        /// <summary>Count of active loans at report date — key underwriting input.</summary>
        public int ActiveLoansCount { get; set; }

        /// <summary>
        /// Days Past Due count — primary delinquency signal.
        /// A single DPD-90+ entry triggers hard rule rejection in the LOS.
        /// </summary>
        public int DpdCount { get; set; }

        /// <summary>FK → DocumentVault entry holding the full bureau PDF. Null until async storage completes.</summary>
        public Guid? RawReportVaultId { get; set; }

        /// <summary>
        /// AI-generated score improvement tips as JSONB array.
        /// Shape: [{ tip: string, impact: "HIGH"|"MEDIUM"|"LOW", actionable: bool }]
        /// </summary>
        public List<Dictionary<string, object>> ImprovementTips { get; set; } = new();

        // Navigation
        public DocumentVault? RawReport { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (ConsentId == Guid.Empty)
                yield return "Bureau pull must reference a ConsentLog entry (CICRA mandate).";

            if (ReportDate > DateTime.UtcNow)
                yield return "ReportDate cannot be in the future.";

            if (DpdCount < 0)
                yield return "DpdCount cannot be negative.";

            if (ActiveLoansCount < 0)
                yield return "ActiveLoansCount cannot be negative.";

            if (NextRefreshDate.HasValue && NextRefreshDate <= ReportDate)
                yield return "NextRefreshDate must be after ReportDate.";
        }

        public override string GetDisplayName() => $"Credit[{Bureau}|DPD:{DpdCount}]:{Id}";
    }
}
