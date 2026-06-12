using Business.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// An individual promoter (director / partner / proprietor / guarantor) of a BusinessProfile.
    ///
    /// DESIGN RULES:
    ///   - PanNumber is AES-256 encrypted PII — never expose in query projections or logs.
    ///   - PersonalProfileId links to the Personal vertical if the promoter is also a DigifinLabs
    ///     individual user. Null for promoters who have not self-registered.
    ///   - A Proprietorship must have exactly one Promoter with Role = Proprietor.
    ///     This cross-entity rule is enforced at application service level, not here.
    ///   - ShareholdingPercent is null for non-equity roles (Guarantor, Director without shares).
    /// </summary>
    public sealed class Promoter : BaseEncryptedEntity
    {
        public required Guid BusinessProfileId { get; init; }

        /// <summary>FK → Personal vertical PersonalProfile.Id. Null if promoter not self-registered.</summary>
        public Guid? PersonalProfileId { get; set; }

        public required string FullName { get; set; }

        public required PromoterRole Role { get; set; }

        /// <summary>
        /// Director Identification Number — MCA-registered companies only.
        /// Null for proprietorships and partnerships.
        /// </summary>
        public string? Din { get; set; }

        /// <summary>Personal PAN, AES-256 encrypted. Required for loan guarantee and AML checks.</summary>
        public required EncryptedValue PanNumber { get; init; }

        /// <summary>Shareholding percentage (0–100). Null for non-equity roles.</summary>
        public decimal? ShareholdingPercent { get; set; }

        public bool IsGuarantor { get; set; }

        public DateOnly? DateOfAppointment { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessProfileId == Guid.Empty)
                yield return "BusinessProfileId must not be empty.";

            if (string.IsNullOrWhiteSpace(FullName))
                yield return "Promoter FullName is required.";

            if (ShareholdingPercent is < 0m or > 100m)
                yield return "ShareholdingPercent must be between 0 and 100.";

            if (Role == PromoterRole.Director && string.IsNullOrWhiteSpace(Din))
                yield return "DIN is required for Directors.";
        }

        public override string GetDisplayName() => $"Promoter[{Role}|{FullName}]:{Id}";
    }
}
