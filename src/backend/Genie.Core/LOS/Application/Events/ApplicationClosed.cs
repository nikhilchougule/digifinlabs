using SharedKernel.Common;
using System;

namespace Los.Application.Events
{
    /// <summary>
    /// Published when an application reaches a terminal state. Routing key: application.closed
    /// Consumers: notification, billing (on disbursed), audit.
    /// </summary>
    public sealed class ApplicationClosed : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }

        /// <summary>Terminal decision: Sanctioned | Rejected | Disbursed | Closed.</summary>
        public required string Decision { get; init; }

        public DateTime ClosedAt { get; init; } = DateTime.UtcNow;
    }
}
