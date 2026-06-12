using System;

namespace Personal.Snapshots
{
    /// <summary>
    /// Read-only snapshot of an individual applicant, captured at write time and stored as JSONB by
    /// consuming services (e.g. an Application's applicant_snapshot when ApplicantType = Individual).
    ///
    /// Mirrors Business.Snapshots.BusinessApplicantSnapshot so the LOS can hold a uniform applicant
    /// copy regardless of applicant type. Display-safe fields only — never plaintext PII.
    /// </summary>
    public sealed record PersonalApplicantSnapshot
    {
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }

        /// <summary>Coarse employment context for display/eligibility (no raw income).</summary>
        public string? EmploymentType { get; init; }

        /// <summary>Latest known bureau score band at snapshot time (null if not pulled).</summary>
        public int? CreditScore { get; init; }

        public required DateTime SnapshottedAt { get; init; }
    }
}
