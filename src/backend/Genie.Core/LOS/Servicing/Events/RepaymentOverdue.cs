using SharedKernel.Common;
using System;

namespace Los.Servicing.Events
{
    /// <summary>
    /// Published when an installment crosses into overdue. Routing key: servicing.repayment.overdue
    /// Consumers: personal (update CreditProfile.DpdCount), risk, notification, audit.
    /// This is the servicing → origination DPD feedback loop.
    /// </summary>
    public sealed class RepaymentOverdue : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public required Guid RepaymentId { get; init; }
        public int DpdDays { get; init; }
        public long OutstandingInPaise { get; init; }
    }
}
