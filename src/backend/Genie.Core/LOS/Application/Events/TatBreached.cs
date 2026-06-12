using Los.Application.Enums;
using SharedKernel.Common;
using System;

namespace Los.Application.Events
{
    /// <summary>
    /// Published when an application breaches its SLA. Routing key: application.tat.breached
    /// Consumers: notification, escalation, audit.
    /// </summary>
    public sealed class TatBreached : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public required ApplicationStage Stage { get; init; }
        public int SlaHours { get; init; }
        public decimal ElapsedHours { get; init; }
    }
}
