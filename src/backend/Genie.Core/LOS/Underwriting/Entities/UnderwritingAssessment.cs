using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Underwriting.Entities
{
    /// <summary>
    /// The underwriting record for an Application (1:1) — holds computed ratios and the credit recommendation.
    ///
    /// DESIGN RULES:
    ///   - References to analyses (banking/gst/income/bureau) are UUID only — no cross-service FK.
    ///   - Ratios are computed by the service layer and stored for reporting/audit.
    ///   - RecommendedAmount uses Money; RecommendedRate is a decimal fraction.
    /// </summary>
    public sealed class UnderwritingAssessment : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required Guid UnderwriterId { get; set; }

        public Guid? IncomeAssessmentId { get; set; }
        public Guid? BankingAnalysisId { get; set; }
        public Guid? GstAnalysisId { get; set; }
        public Guid? BureauCreditProfileId { get; set; }

        // ── Key ratios (computed) ──
        public decimal? Dscr { get; set; }              // Debt Service Coverage Ratio
        public decimal? Ltv { get; set; }               // Loan-to-Value
        public decimal? Foir { get; set; }              // Fixed Obligation to Income Ratio
        public decimal? TurnoverMultiple { get; set; }
        public decimal? LeverageRatio { get; set; }
        public decimal? CurrentRatio { get; set; }
        public decimal? EmiToIncomeRatio { get; set; }

        // ── Recommendation ──
        public Money? RecommendedAmount { get; set; }
        public int? RecommendedTenure { get; set; }
        public decimal? RecommendedRate { get; set; }
        public bool CollateralRequirement { get; set; }
        public Dictionary<string, object> CollateralDetails { get; set; } = new();

        public string? UnderwriterNotes { get; set; }
        public Dictionary<string, object> ExceptionFlags { get; set; } = new();
        public DateTime? SubmittedAt { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (UnderwriterId == Guid.Empty)
                yield return "UnderwriterId must not be empty.";

            if (RecommendedTenure is <= 0 or > 360)
                yield return "RecommendedTenure, when set, must be between 1 and 360.";
        }

        public override string GetDisplayName() => $"Underwriting:{ApplicationId}";
    }
}
