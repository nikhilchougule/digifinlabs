using Los.Application.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// The LOS root aggregate — a single credit application / case, from lead through to closure.
    /// (Formerly "Case"; renamed to Application. The internal staff view is the same record.)
    ///
    /// DESIGN RULES:
    ///   - ApplicantId is polymorphic (PersonalProfile.Id | BusinessProfile.Id), paired with ApplicantType.
    ///     UUID reference only — no cross-service FK.
    ///   - ApplicantSnapshot is a frozen JSONB copy of the applicant captured at creation
    ///     (Personal/BusinessApplicantSnapshot) so reads never hit the owning vertical.
    ///   - Four-eye principle: Sales ≠ Credit ≠ Risk on the same application (Validate + DB CHECK later).
    ///   - TAT clock starts at DocumentCollection (first document) and ends at the risk decision.
    ///   - All monetary values use the Money value object (paise).
    /// </summary>
    public sealed class Application : BaseAuditableEntity
    {
        /// <summary>Human-readable reference, e.g. DFL-2026-000042.</summary>
        public required string ApplicationNumber { get; init; }

        public required ApplicationType ApplicationType { get; init; }

        public required ApplicantType ApplicantType { get; init; }

        /// <summary>PersonalProfile.Id or BusinessProfile.Id. UUID reference — no FK.</summary>
        public required Guid ApplicantId { get; init; }

        /// <summary>Frozen applicant copy at creation (Personal/BusinessApplicantSnapshot as JSONB).</summary>
        public Dictionary<string, object> ApplicantSnapshot { get; init; } = new();

        public ApplicationStage CurrentStage { get; set; } = ApplicationStage.Lead;

        // ── Four-eye staff assignment (UUID refs to identity-service) ──
        public Guid? AssignedSalesId { get; set; }
        public Guid? AssignedCreditId { get; set; }
        public Guid? AssignedRiskId { get; set; }

        /// <summary>Lender product selected after matching. UUID ref to Lending context — no FK.</summary>
        public Guid? LenderProductId { get; set; }

        // ── Financials ──
        public Money? LoanAmountRequested { get; set; }
        public Money? LoanAmountSanctioned { get; set; }
        public int? TenureMonths { get; set; }
        public decimal? InterestRateOffered { get; set; } // decimal fraction: 0.115 = 11.5% p.a.

        // ── TAT ──
        public DateTime? TatStartAt { get; set; }
        public DateTime? TatEndAt { get; set; }
        public int? TatSlaHours { get; set; }
        public bool TatBreachFlag { get; set; }

        public AiRiskFlag AiRiskFlag { get; set; } = AiRiskFlag.Green;
        public ApplicationPriority Priority { get; set; } = ApplicationPriority.Normal;
        public ApplicationSource Source { get; set; } = ApplicationSource.Sales;

        public DateTime? SubmittedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        /// <summary>Stage-wise internal notes (JSONB).</summary>
        public Dictionary<string, object> Remarks { get; set; } = new();

        // Navigation (in-aggregate)
        public ApplicationProduct? Product { get; set; }
        public List<ApplicationDocument> Documents { get; set; } = new();
        public List<ApplicationStageLog> StageHistory { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (string.IsNullOrWhiteSpace(ApplicationNumber))
                yield return "ApplicationNumber is required.";

            if (ApplicantId == Guid.Empty)
                yield return "ApplicantId must not be empty.";

            // Four-eye principle (RBI governance)
            if (AssignedSalesId is { } s && (s == AssignedCreditId || s == AssignedRiskId))
                yield return "Four-eye rule: Sales cannot also be Credit or Risk on the same application.";
            if (AssignedCreditId is { } c && c == AssignedRiskId)
                yield return "Four-eye rule: Credit cannot also be Risk on the same application.";

            if (CurrentStage is ApplicationStage.Closed && ClosedAt is null)
                yield return "ClosedAt must be set when the application is Closed.";

            if (LoanAmountSanctioned is not null && LoanAmountSanctioned.AmountInPaise < 0)
                yield return "LoanAmountSanctioned cannot be negative.";
        }

        public override string GetDisplayName() => $"Application[{ApplicationNumber}|{CurrentStage}]:{Id}";
    }
}
