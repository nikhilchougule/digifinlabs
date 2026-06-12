using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// A single ordered step within a Workflow.
    ///
    /// DESIGN RULES:
    ///   - Input / Output are opaque JSONB — the workflow engine does not interpret them.
    ///   - AssignedToId is set only for manual steps (ManualReview / Decision); UUID ref, no FK.
    ///   - RetryCount tracks automatic retries for transient step failures.
    /// </summary>
    public sealed class WorkflowStep : BaseEntity
    {
        public required Guid WorkflowId { get; init; }

        public required int StepOrder { get; init; }

        public required WorkflowStepType StepType { get; init; }

        public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.Pending;

        public Dictionary<string, object> Input { get; set; } = new();

        public Dictionary<string, object> Output { get; set; } = new();

        /// <summary>Assignee for manual steps. References identity-service by UUID — no FK.</summary>
        public Guid? AssignedToId { get; set; }

        public DateTime? DueAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int RetryCount { get; set; } = 0;

        // Navigation (in-aggregate)
        public Workflow? Workflow { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (WorkflowId == Guid.Empty)
                yield return "WorkflowId must not be empty.";

            if (StepOrder < 0)
                yield return "StepOrder cannot be negative.";

            if (RetryCount < 0)
                yield return "RetryCount cannot be negative.";

            if (CompletedAt.HasValue && StartedAt.HasValue && CompletedAt < StartedAt)
                yield return "CompletedAt cannot be before StartedAt.";
        }

        public override string GetDisplayName() => $"WorkflowStep[{StepOrder}:{StepType}|{Status}]:{Id}";
    }
}
