using SharedKernel.Common;
using System;
using System.Collections.Generic;

namespace Business.Entities
{
    /// <summary>
    /// GST footprint of a business — the aggregate root for its filing history.
    ///
    /// DESIGN RULES:
    ///   - 1:1 with BusinessProfile.
    ///   - GstScore is a filing-regularity score (0–100) recomputed on each sync — never edited.
    ///   - RawData holds the verbatim GSTN API response (JSONB) for audit/reprocessing.
    ///   - ConsentExpiry is enforced upstream: no GST pull without a valid ConsentLog (GST_DATA).
    /// </summary>
    public sealed class GstProfile : BaseAuditableEntity
    {
        public required Guid BusinessId { get; init; }

        /// <summary>15-character GSTIN (plaintext key for filing lookups; the encrypted copy lives on BusinessProfile).</summary>
        public required string Gstin { get; set; }

        /// <summary>Filing-regularity score (0–100). Null until first sync.</summary>
        public decimal? GstScore { get; set; }

        public DateTime? LastSyncedAt { get; set; }

        /// <summary>Verbatim GSTN API response (JSONB) — stored for audit and reprocessing.</summary>
        public Dictionary<string, object> RawData { get; set; } = new();

        // Navigation (in-aggregate)
        public List<GstFiling> Filings { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (BusinessId == Guid.Empty)
                yield return "BusinessId must not be empty.";

            if (string.IsNullOrWhiteSpace(Gstin) || Gstin.Length != 15)
                yield return "GSTIN must be exactly 15 characters.";

            if (GstScore is < 0m or > 100m)
                yield return "GstScore must be between 0 and 100.";
        }

        public override string GetDisplayName() => $"GstProfile[{Gstin}]:{Id}";
    }
}
