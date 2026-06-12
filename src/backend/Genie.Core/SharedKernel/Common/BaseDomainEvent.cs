using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    /// <summary>
    /// Base type for every domain event published to the RabbitMQ event bus.
    /// EventId is the idempotency key consumers dedup on; OccurredAt is the logical event time.
    /// Version increments on payload changes for backward-compatible consumption of older events.
    /// </summary>
    public abstract class BaseDomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid(); //Idempotency key — consumers dedup on this. Set once on creation.
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow; //Logical time the event occurred. Set once on creation.
        public int Version { get; init; } = 1; //Default 1, increment on payload changes to allow versioning of domain events and backward compatibility when processing older events with newer code.
    }
}
