using Los.Application.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Lending.Entities
{
    /// <summary>
    /// A product in a lender's catalogue — the unit applications are matched against.
    ///
    /// DESIGN RULES:
    ///   - ApplicableTypes lists which ApplicationTypes this product covers.
    ///   - Min/Max amounts use Money; rates are decimal fractions (0.09 = 9% p.a.).
    ///   - EligibilityCriteria is a typed RuleCondition list (compile-checked DSL), evaluated by the
    ///     lender-matching engine — the structured counterpart of the eligibility JSONB.
    /// </summary>
    public sealed class LenderProduct : BaseAuditableEntity
    {
        public required Guid LenderId { get; init; }

        public required string Name { get; set; }

        public List<ApplicationType> ApplicableTypes { get; set; } = new();

        public required Money MinAmount { get; set; }
        public required Money MaxAmount { get; set; }

        /// <summary>Decimal fraction, e.g. 0.09 = 9% p.a.</summary>
        public required decimal MinRate { get; set; }
        public required decimal MaxRate { get; set; }

        public required int MinTenureMonths { get; set; }
        public required int MaxTenureMonths { get; set; }

        /// <summary>Typed eligibility predicates evaluated during matching.</summary>
        public List<RuleCondition> EligibilityCriteria { get; set; } = new();

        public bool IsActive { get; set; } = true;

        // Navigation (in-aggregate)
        public Lender? Lender { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (LenderId == Guid.Empty)
                yield return "LenderId must not be empty.";

            if (string.IsNullOrWhiteSpace(Name))
                yield return "LenderProduct Name is required.";

            if (MinAmount.AmountInPaise > MaxAmount.AmountInPaise)
                yield return "MinAmount cannot exceed MaxAmount.";

            if (MinRate <= 0 || MaxRate > 1 || MinRate > MaxRate)
                yield return "Rates must be decimal fractions in (0,1] with MinRate ≤ MaxRate.";

            if (MinTenureMonths <= 0 || MinTenureMonths > MaxTenureMonths)
                yield return "MinTenureMonths must be positive and ≤ MaxTenureMonths.";

            foreach (var c in EligibilityCriteria)
                foreach (var err in c.Validate())
                    yield return err;
        }

        public override string GetDisplayName() => $"LenderProduct[{Name}]:{Id}";
    }
}
