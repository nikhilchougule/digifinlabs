using Business.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// A CA-certified financial statement for a fiscal year — the formal income evidence for MSME lending.
    ///
    /// DESIGN RULES:
    ///   - VaultId is mandatory once uploaded — the PDF lives in DocumentVault, not here.
    ///   - Monetary fields (TotalRevenue, NetProfit, etc.) are nullable — populated either by
    ///     AI extraction from the PDF or by manual entry during underwriting. Both paths are valid.
    ///   - ExtractedData holds the raw AI extraction output (JSONB). The structured Money fields are
    ///     the normalised version of that output and are what underwriting rules operate on.
    ///   - Multiple statement types per fiscal year are allowed (BalanceSheet + P&L + CashFlow).
    /// </summary>
    public sealed class FinancialStatement : BaseAuditableEntity
    {
        public required Guid BusinessProfileId { get; init; }

        /// <summary>Fiscal year string. Format: "YYYY-YY" e.g. "2023-24".</summary>
        public required string FiscalYear { get; init; }

        public required FinancialStatementType StatementType { get; init; }

        public bool IsAudited { get; set; }

        public string? AuditorName { get; set; }

        /// <summary>FK → DocumentVault holding the CA-certified PDF. Mandatory for audited statements.</summary>
        public required Guid VaultId { get; init; }

        // ── AI-extracted / manually keyed financials ──

        public Money? TotalRevenue { get; set; }

        public Money? NetProfit { get; set; }

        public Money? TotalAssets { get; set; }

        public Money? TotalLiabilities { get; set; }

        /// <summary>Earnings Before Interest, Tax, Depreciation, and Amortisation.</summary>
        public Money? Ebitda { get; set; }

        /// <summary>
        /// Full raw AI extraction output as JSONB.
        /// Null until the AI pipeline processes the VaultId document.
        /// The structured Money fields above are normalised from this output.
        /// </summary>
        public Dictionary<string, object> ExtractedData { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessProfileId == Guid.Empty)
                yield return "BusinessProfileId must not be empty.";

            if (string.IsNullOrWhiteSpace(FiscalYear) ||
                !System.Text.RegularExpressions.Regex.IsMatch(FiscalYear, @"^\d{4}-\d{2}$"))
                yield return "FiscalYear must follow the format YYYY-YY (e.g. 2023-24).";

            if (VaultId == Guid.Empty)
                yield return "VaultId must reference the uploaded document in DocumentVault.";

            if (IsAudited && string.IsNullOrWhiteSpace(AuditorName))
                yield return "AuditorName is required for audited financial statements.";
        }

        public override string GetDisplayName() => $"FinStmt[{StatementType}|{FiscalYear}]:{Id}";
    }
}
