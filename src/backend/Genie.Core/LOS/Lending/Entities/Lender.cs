using Los.Lending.Enums;
using SharedKernel.Common;
using System;
using System.Collections.Generic;

namespace Los.Lending.Entities
{
    /// <summary>
    /// A lending partner (NBFC / Bank / HFC) whose products applications can be matched to.
    ///
    /// DESIGN RULES:
    ///   - ApiEndpoint is set when the lender supports automated loan pass-through.
    ///   - Disbursed loans against this lender's products drive facilitation-fee billing.
    /// </summary>
    public sealed class Lender : BaseAuditableEntity
    {
        public required string Name { get; set; }

        public required string LicenceNumber { get; set; }

        public required LenderType LenderType { get; init; }

        /// <summary>Base URL for automated loan pass-through. Null for manual lenders.</summary>
        public string? ApiEndpoint { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation (in-aggregate)
        public List<LenderProduct> Products { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (string.IsNullOrWhiteSpace(Name))
                yield return "Lender Name is required.";

            if (string.IsNullOrWhiteSpace(LicenceNumber))
                yield return "LicenceNumber is required (RBI registration).";
        }

        public override string GetDisplayName() => $"Lender[{LenderType}|{Name}]:{Id}";
    }
}
