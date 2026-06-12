using Los.Application.Enums;
using SharedKernel.Common;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// Immutable audit trail of every stage transition on an Application.
    ///
    /// DESIGN RULES:
    ///   - APPEND-ONLY (IAppendOnly): no UPDATE or DELETE — enforced by a DB trigger + EF interceptor.
    ///     The regulatory audit trail cannot have gaps or modifications.
    ///   - All properties are init-only to reinforce immutability at the domain layer.
    /// </summary>
    /// 
    public sealed class ApplicationStageLog : BaseEntity, IAppendOnly
    {
        public required Guid ApplicationId { get; init; }

        public ApplicationStage? FromStage { get; init; }

        public required ApplicationStage ToStage { get; init; }

        public required Guid TransitionedById { get; init; }

        public DateTime TransitionedAt { get; init; } = DateTime.UtcNow;

        /// <summary>Hours spent in FromStage before this transition (computed at write time).</summary>
        public decimal? TimeSpentHours { get; init; }

        public string? ReasonCode { get; init; }

        public string? Notes { get; init; }

        public bool SlaBreached { get; init; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (TransitionedById == Guid.Empty)
                yield return "TransitionedById must not be empty.";

            if (FromStage == ToStage)
                yield return "A stage log must record a change (FromStage must differ from ToStage).";
        }

        public override string GetDisplayName() => $"StageLog[{FromStage}→{ToStage}]:{Id}";
    }
}
