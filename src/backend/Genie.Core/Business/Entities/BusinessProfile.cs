using Business.Enums;
using SharedKernel.Common;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// Profile of an MSME on the DigifinLabs Business vertical.
    ///
    /// DESIGN RULES:
    ///   - 1:1 with the promoter User — UserId mirrors User.ProfileRefId.
    ///   - GSTIN and PAN are PII — stored as AES-256 EncryptedValue. UI shows masked forms.
    ///     Decrypt only within a trusted service boundary; never in query projections or logs.
    ///   - KycProfileId references the kyc-service by UUID (no cross-service FK). Business-level
    ///     KYC status is mirrored locally via the kyc.verification.passed event.
    ///   - Address reuses the SharedKernel Address value object (stored as an EF owned entity).
    /// </summary>
    public sealed class BusinessProfile : BaseEncryptedEntity
    {
        /// <summary>Owner / promoter. References identity-service by UUID — no FK.</summary>
        public required Guid UserId { get; init; }

        public required string LegalName { get; set; }

        public string? TradeName { get; set; }

        public required BusinessEntityType EntityType { get; set; }

        /// <summary>15-character GSTIN, AES-256 encrypted. UI shows masked form.</summary>
        public EncryptedValue? Gstin { get; set; }

        /// <summary>Business PAN, AES-256 encrypted.</summary>
        public EncryptedValue? Pan { get; set; }

        /// <summary>Udyam (MSME) registration number. Null until registered.</summary>
        public string? UdyamNumber { get; set; }

        public DateOnly? IncorporationDate { get; set; }

        /// <summary>NIC industry code (2–5 digits). Drives sector concentration and underwriting benchmarks.</summary>
        public string? IndustryCode { get; set; }

        /// <summary>Registered business address. Stored as an EF Core owned entity.</summary>
        public Address? Address { get; set; }

        public TurnoverBand? AnnualTurnoverBand { get; set; }

        /// <summary>FK → most recent business-level KycProfile in kyc-service. UUID reference, no FK.</summary>
        public Guid? KycProfileId { get; set; }

        /// <summary>Local mirror of business KYC state, updated via the kyc.verification.passed event.</summary>
        public KycStatus KycStatus { get; set; } = KycStatus.Pending;

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (string.IsNullOrWhiteSpace(LegalName))
                yield return "LegalName is required.";

            if (IncorporationDate.HasValue && IncorporationDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                yield return "IncorporationDate cannot be in the future.";

            if (!string.IsNullOrWhiteSpace(IndustryCode) && (IndustryCode.Length < 2 || IndustryCode.Length > 5))
                yield return "IndustryCode must be a 2–5 digit NIC code.";
        }

        public override string GetDisplayName() => $"Business[{EntityType}|{LegalName}]:{Id}";
    }
}
