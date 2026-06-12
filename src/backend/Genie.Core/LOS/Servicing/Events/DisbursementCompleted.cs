using SharedKernel.Common;
using System;

namespace Los.Servicing.Events
{
    /// <summary>
    /// Published when a tranche is disbursed. Routing key: servicing.disbursement.completed
    /// Consumers: application (advance to Disbursed), billing (facilitation fee), notification, audit.
    /// </summary>
    public sealed class DisbursementCompleted : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public required Guid DisbursementId { get; init; }
        public int TrancheNumber { get; init; }
        public long AmountInPaise { get; init; }
    }
}
