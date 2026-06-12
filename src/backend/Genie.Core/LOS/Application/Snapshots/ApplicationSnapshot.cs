using Los.Application.Enums;
using System;

namespace Los.Application.Snapshots
{
    /// <summary>
    /// Read-only snapshot of an Application's headline fields, for services that display application
    /// context without owning it (e.g. notification, billing). Stored as JSONB by the consumer.
    /// </summary>
    public sealed record ApplicationSnapshot
    {
        public required Guid ApplicationId { get; init; }
        public required string ApplicationNumber { get; init; }
        public required ApplicationType ApplicationType { get; init; }
        public required ApplicationStage CurrentStage { get; init; }
        public long? SanctionedAmountInPaise { get; init; }
        public required DateTime SnapshottedAt { get; init; }
    }
}
