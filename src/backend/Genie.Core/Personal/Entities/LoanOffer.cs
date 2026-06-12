using Personal.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// A pre-qualified loan offer presented to a user via the Loan Discovery Engine.
    ///
    /// KEY DIFFERENTIATOR:
    ///   EligibilityScore is computed from soft signals (income, AA bank data, internal scoring)
    ///   WITHOUT a hard bureau enquiry. No CIBIL hit until the user explicitly applies (Shown → Applied).
    ///   This protects the user's bureau score — the core differentiator vs BankBazaar / PaisaBazaar.
    ///
    /// DESIGN RULES:
    ///   - InterestRate is stored as a decimal fraction: 0.1150 = 11.50% p.a.
    ///     Never store as integer percentage — causes rounding errors in EMI computation.
    ///   - UtmRef maps this offer to a revenue event — Disbursed status triggers
    ///     loan facilitation fee billing (0.5%–2.0% per the revenue model).
    ///   - LenderId references the Lender entity (built in the API Suite phase).
    /// </summary>
    public sealed class LoanOffer : BaseAuditableEntity
    {
        public required Guid UserId { get; init; }

        /// <summary>FK → Lender entity (API Suite phase). Identifies the lending partner.</summary>
        public required Guid LenderId { get; init; }

        public required LoanProductType ProductType { get; init; }

        public required Money OfferedAmount { get; init; }

        /// <summary>
        /// Annual interest rate as a decimal fraction.
        /// Examples: 0.1150 = 11.50% p.a. | 0.1800 = 18.00% p.a.
        /// </summary>
        public required decimal InterestRate { get; init; }

        public required int TenureMonths { get; init; }

        /// <summary>
        /// DigifinLabs pre-qualification score (0–100) derived from soft signals.
        /// Computed WITHOUT a hard bureau enquiry — protects user's CIBIL score.
        /// </summary>
        public required decimal EligibilityScore { get; init; }

        public required DateTime OfferExpiry { get; init; }

        public LoanOfferStatus Status { get; set; } = LoanOfferStatus.Shown;

        /// <summary>Revenue attribution reference for loan facilitation fee billing.</summary>
        public string? UtmRef { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (LenderId == Guid.Empty)
                yield return "LenderId must not be empty.";

            if (OfferedAmount.AmountInPaise <= 0)
                yield return "OfferedAmount must be greater than zero.";

            if (InterestRate <= 0 || InterestRate > 1)
                yield return "InterestRate must be a decimal fraction between 0 and 1 (e.g. 0.115 for 11.5% p.a.).";

            if (TenureMonths <= 0)
                yield return "TenureMonths must be greater than zero.";

            if (TenureMonths > 360)
                yield return "TenureMonths cannot exceed 360 (30 years).";

            if (EligibilityScore < 0 || EligibilityScore > 100)
                yield return "EligibilityScore must be between 0 and 100.";

            if (OfferExpiry <= DateTime.UtcNow)
                yield return "OfferExpiry must be in the future.";
        }

        public override string GetDisplayName() => $"Offer[{ProductType}|{Status}]:{Id}";
    }
}
