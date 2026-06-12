using SharedKernel.Common;
using SharedKernel.Enums;
using System;

namespace Business.Events
{
    /// <summary>
    /// Published when a new BusinessCreditScore is computed.
    /// Routing key: business.score.computed
    /// Consumers: social-service (verified badge / score snapshot), loan-offer, scoring, audit.
    /// </summary>
    public sealed class BusinessScoreComputed : BaseDomainEvent
    {
        public required Guid BusinessId { get; init; }
        public required decimal Score { get; init; }
        public required RiskBand RiskBand { get; init; }

        /// <summary>ScorecardVersion that produced this score — mandatory for governance.</summary>
        public required string ScorecardVersion { get; init; }
    }
}
