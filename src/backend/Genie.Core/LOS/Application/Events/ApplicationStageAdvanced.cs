using Los.Application.Enums;
using SharedKernel.Common;
using System;

namespace Los.Application.Events
{
    /// <summary>
    /// Published on every stage transition. Routing key: application.stage.advanced
    /// Consumers: notification, tat, audit (and stage-specific triggers in risk/underwriting).
    /// </summary>
    public sealed class ApplicationStageAdvanced : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public ApplicationStage? FromStage { get; init; }
        public required ApplicationStage ToStage { get; init; }
        public required Guid ByUserId { get; init; }
    }
}
