using SharedKernel.Common;
using System;

namespace Business.Events
{
    /// <summary>
    /// Published when a new MSME profile is created.
    /// Routing key: business.profile.created
    /// Consumers: social-service (create social presence), workflow-service, audit-service.
    /// </summary>
    public sealed class BusinessProfileCreated : BaseDomainEvent
    {
        public required Guid BusinessId { get; init; }
        public required Guid UserId { get; init; }

        /// <summary>Masked GSTIN — never publish the plaintext PII over the bus.</summary>
        public string? MaskedGstin { get; init; }
    }
}
