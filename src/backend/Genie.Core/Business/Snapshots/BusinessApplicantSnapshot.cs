using Business.Enums;
using SharedKernel.Enums;
using System;

namespace Business.Snapshots
{
    /// <summary>
    /// Read-only snapshot of a business applicant, captured at write time and stored as JSONB by
    /// consuming services (e.g. case-service.cases.applicant_snapshot).
    ///
    /// Per the denormalisation strategy: the consumer calls business-service ONCE at creation,
    /// stores this snapshot, and never makes a cross-service call on the read path again.
    /// Carries only display-safe fields — masked GSTIN, never plaintext PII.
    /// </summary>
    public sealed record BusinessApplicantSnapshot
    {
        public required Guid BusinessId { get; init; }
        public required string LegalName { get; init; }
        public string? MaskedGstin { get; init; }
        public required BusinessEntityType EntityType { get; init; }

        /// <summary>Latest known risk band at snapshot time (null if not yet scored).</summary>
        public RiskBand? RiskBand { get; init; }

        /// <summary>Latest known score (0–1000) at snapshot time (null if not yet scored).</summary>
        public decimal? Score { get; init; }

        public required DateTime SnapshottedAt { get; init; }
    }
}
