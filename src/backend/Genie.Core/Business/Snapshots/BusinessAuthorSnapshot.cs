using System;

namespace Business.Snapshots
{
    /// <summary>
    /// Lightweight business identity snapshot for the MSME social network
    /// (e.g. social-service.msme_posts.author_snapshot, listings author display).
    ///
    /// Updated on business.profile.created and refreshed on business.score.computed /
    /// kyc.verification.passed events so the social feed never calls business-service at read time.
    /// </summary>
    public sealed record BusinessAuthorSnapshot
    {
        public required Guid BusinessId { get; init; }
        public required string BusinessName { get; init; }

        /// <summary>True once business KYC is verified — drives the "verified" badge.</summary>
        public bool Verified { get; init; }

        /// <summary>NIC 2-digit industry code for sector-based discovery.</summary>
        public string? IndustryCode { get; init; }
    }
}
