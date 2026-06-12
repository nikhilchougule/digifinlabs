using Los.Application.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// What the applicant is asking for — one per Application.
    ///
    /// DESIGN RULES:
    ///   - RequestedAmount uses the Money value object.
    ///   - TenureMonths is null for revolving facilities (working capital) and stress resolution.
    ///   - Parameters is a type-specific JSONB bag, e.g.
    ///       WorkingCapital:       { limitType, drawdownPeriod }
    ///       MsmeStressResolution: { existingLoanRefs, restructureType }
    ///       LoanAgainstProperty:  { propertyType, estimatedValue }
    /// </summary>
    public sealed class ApplicationProduct : BaseEntity
    {
        public required Guid ApplicationId { get; init; }

        public required Money RequestedAmount { get; set; }

        public string? PurposeDescription { get; set; }

        public int? TenureMonths { get; set; }

        public DisbursementMode? DisbursementMode { get; set; }

        public RepaymentFrequency? RepaymentFrequency { get; set; }

        /// <summary>Type-specific fields keyed by ApplicationType (JSONB).</summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (RequestedAmount.AmountInPaise <= 0)
                yield return "RequestedAmount must be greater than zero.";

            if (TenureMonths is <= 0 or > 360)
                yield return "TenureMonths, when set, must be between 1 and 360.";
        }

        public override string GetDisplayName() => $"ApplicationProduct[{RequestedAmount}]:{Id}";
    }
}
