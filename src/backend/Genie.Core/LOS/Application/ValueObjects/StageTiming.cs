using Los.Application.Enums;
using System;

namespace Los.Application.ValueObjects
{
    /// <summary>
    /// One entry in a TatTracker's stage-timing series (typed counterpart of the stage_timings JSONB).
    /// Captures how long the application spent in a given stage and whether the per-stage SLA was breached.
    /// </summary>
    public sealed record StageTiming
    {
        public required ApplicationStage Stage { get; init; }
        public required DateTime EnteredAt { get; init; }
        public DateTime? ExitedAt { get; init; }
        public decimal? DurationHours { get; init; }
        public required int SlaHours { get; init; }
        public bool Breached { get; init; }
        public string? BottleneckReason { get; init; }
    }
}
