using Business.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Business.Entities
{
    /// <summary>
    /// A single GST return for one period — the unit of GST filing history.
    ///
    /// DESIGN RULES:
    ///   - Unique per (GstProfileId, ReturnType, Period) — re-syncs must dedup, not duplicate.
    ///   - Monetary values use the Money value object (paise). Never decimal for storage.
    ///   - DelayDays feeds the filing-consistency score; FiledLate/NotFiled are negative signals.
    /// </summary>
    public sealed class GstFiling : BaseEntity
    {
        public required Guid GstProfileId { get; init; }

        public required GstReturnType ReturnType { get; init; }

        /// <summary>Filing period in MM-YYYY format.</summary>
        public required string Period { get; init; }

        public Money? TaxableTurnover { get; set; }

        public Money? TaxPaid { get; set; }

        public DateOnly? FiledOn { get; set; }

        public required GstFilingStatus FilingStatus { get; set; }

        /// <summary>Days filed past the due date. 0 when on time; positive when late.</summary>
        public int DelayDays { get; set; }

        // Navigation (in-aggregate)
        public GstProfile? Profile { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (GstProfileId == Guid.Empty)
                yield return "GstProfileId must not be empty.";

            if (string.IsNullOrWhiteSpace(Period) || !Regex.IsMatch(Period, "^(0[1-9]|1[0-2])-\\d{4}$"))
                yield return "Period must be in MM-YYYY format.";

            if (DelayDays < 0)
                yield return "DelayDays cannot be negative.";

            if (FilingStatus == GstFilingStatus.FiledLate && DelayDays <= 0)
                yield return "A FiledLate return must have DelayDays greater than zero.";
        }

        public override string GetDisplayName() => $"GstFiling[{ReturnType}|{Period}]:{Id}";
    }
}
