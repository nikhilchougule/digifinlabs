using Los.Application.Enums;
using Los.Application.ValueObjects;
using SharedKernel.Common;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// Turn-around-time tracker for an Application — one per application.
    ///
    /// DESIGN RULES:
    ///   - StageTimings is a typed series (StageTiming) rather than raw JSONB.
    ///   - BottleneckStage and PredictedCompletionAt are populated by the AI TAT-prediction engine.
    ///   - Drives escalation: when TatStatus reaches AtRisk/Breached, notification-service is triggered.
    /// </summary>
    public sealed class TatTracker : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required int ProductSlaHours { get; set; }

        public List<StageTiming> StageTimings { get; set; } = new();

        public decimal? TotalElapsedHours { get; set; }

        public decimal? SlaRemainingHours { get; set; }

        public ApplicationStage? BottleneckStage { get; set; }

        public TatStatus TatStatus { get; set; } = TatStatus.OnTrack;

        public DateTime? PredictedCompletionAt { get; set; }

        public bool EscalationTriggered { get; set; }

        public Guid? EscalatedToId { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (ProductSlaHours <= 0)
                yield return "ProductSlaHours must be greater than zero.";
        }

        public override string GetDisplayName() => $"TatTracker[{TatStatus}]:{ApplicationId}";
    }
}
