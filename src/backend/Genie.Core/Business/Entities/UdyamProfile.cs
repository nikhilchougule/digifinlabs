using Business.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// Udyam (MSME) registration record for a business.
    ///
    /// DESIGN RULES:
    ///   - UdyamNumber format: UDYAM-XX-00-XXXXXXX (e.g. UDYAM-MH-33-0001234).
    ///   - Classification is derived from the LOWER of InvestmentInMachinery AND AnnualTurnover
    ///     per the revised RBI/MSME criteria (June 2020). It is NOT directly user-entered.
    ///     The application service computes it from the two Money fields.
    ///   - One BusinessProfile can have at most one active UdyamProfile.
    /// </summary>
    public sealed class UdyamProfile : BaseAuditableEntity
    {
        public required Guid BusinessProfileId { get; init; }

        /// <summary>Udyam Registration Number. Format: UDYAM-XX-NN-NNNNNNN.</summary>
        public required string UdyamNumber { get; init; }

        public required DateOnly RegistrationDate { get; init; }

        /// <summary>
        /// Derived classification — computed by application service from InvestmentInMachinery
        /// and AnnualTurnover using revised RBI criteria. Never set directly by the user.
        /// </summary>
        public required MsmeClassification Classification { get; set; }

        /// <summary>Investment in plant and machinery (in paise). RBI classification trigger.</summary>
        public required Money InvestmentInMachinery { get; set; }

        /// <summary>Annual turnover declared at registration (in paise).</summary>
        public required Money AnnualTurnover { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessProfileId == Guid.Empty)
                yield return "BusinessProfileId must not be empty.";

            if (string.IsNullOrWhiteSpace(UdyamNumber))
                yield return "UdyamNumber is required.";

            if (!System.Text.RegularExpressions.Regex.IsMatch(UdyamNumber, @"^UDYAM-[A-Z]{2}-\d{2}-\d{7}$"))
                yield return "UdyamNumber must follow the format UDYAM-XX-NN-NNNNNNN (e.g. UDYAM-MH-33-0001234).";

            if (RegistrationDate > DateOnly.FromDateTime(DateTime.UtcNow))
                yield return "RegistrationDate cannot be in the future.";

            if (InvestmentInMachinery.AmountInPaise < 0)
                yield return "InvestmentInMachinery cannot be negative.";

            if (AnnualTurnover.AmountInPaise < 0)
                yield return "AnnualTurnover cannot be negative.";
        }

        public override string GetDisplayName() => $"Udyam[{Classification}|{UdyamNumber}]:{Id}";
    }
}
