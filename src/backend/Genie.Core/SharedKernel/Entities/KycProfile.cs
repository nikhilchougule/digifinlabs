using SharedKernel.Common;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Entities
{
    /// <summary>
    /// eKYC result record for a User. One record per KYC attempt; the latest verified
    /// record is referenced by User.KycRefId.
    /// All PII fields (Aadhaar, PAN) are AES-256 encrypted — never store or log plaintext.
    /// </summary>
    public sealed class KycProfile : BaseEncryptedEntity
    {
        public required Guid UserId { get; init; }

        /// <summary>Aadhaar number, AES-256 encrypted. Regulated under Aadhaar Act 2016.</summary>
        public required EncryptedValue AadhaarNumber { get; init; }

        /// <summary>PAN number, AES-256 encrypted. Required for income tax linkage.</summary>
        public required EncryptedValue PanNumber { get; init; }

        /// <summary>Face match confidence score from liveness check. Range: 0.0 – 1.0.</summary>
        public decimal FaceMatchScore { get; init; }

        public bool LivenessVerified { get; init; }

        public DateTime? VerifiedAt { get; init; }

        public required KycProvider Provider { get; init; }

        /// <summary>Raw API response from KYC provider stored as JSONB for audit and re-processing.</summary>
        public Dictionary<string, object> RawResponse { get; init; } = new();

        /// <summary>Expiry date for KYC validity (e.g. 1-year rolling window). Null = no expiry.</summary>
        public DateTime? ExpiryDate { get; init; }

        // Navigation
        public User? User { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var error in base.Validate())
                yield return error;

            if (UserId == Guid.Empty)
                yield return "KYC Profile must be associated with a User.";

            if (FaceMatchScore < 0.0m || FaceMatchScore > 1.0m)
                yield return "Face Match Score must be between 0.0 and 1.0.";
        }

        public override string GetDisplayName() => $"KYC[{Provider}|User:{UserId}]:{Id}";
    }
}
