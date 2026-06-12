using SharedKernel.Common;
using System;

namespace Business.Events
{
    /// <summary>
    /// Published when business-level KYC is verified.
    /// Routing key: business.profile.kyc_verified
    /// Consumers: audit-service, plus any vertical gating features on business KYC.
    /// </summary>
    public sealed class BusinessKycVerified : BaseDomainEvent
    {
        public required Guid BusinessId { get; init; }
        public required Guid KycProfileId { get; init; }
    }
}
