using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// A configurable, multi-step workflow instance — shared platform concept.
    /// Used by the Business vertical (onboarding, KYC refresh) and the LOS (loan appraisal, collections).
    ///
    /// DESIGN RULES:
    ///   - EntityId is polymorphic (User | BusinessProfile | Application) with an EntityType discriminator.
    ///     UUID reference only; no cross-service FK.
    ///   - The workflow engine interprets no business logic — step Input/Output are opaque JSONB.
    /// </summary>
    public sealed class Workflow : BaseAuditableEntity
    {
        public required string Name { get; set; }

        public required Guid TenantId { get; init; }

        public required WorkflowType Type { get; init; }

        /// <summary>Polymorphic subject of the workflow.</summary>
        public required Guid EntityId { get; init; }

        /// <summary>Discriminator: 'User' | 'BusinessProfile' | 'Application'.</summary>
        public required string EntityType { get; init; }

        public int CurrentStepIndex { get; set; } = 0;

        public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;

        // Navigation (in-aggregate)
        public List<WorkflowStep> Steps { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (TenantId == Guid.Empty)
                yield return "TenantId must not be empty.";

            if (EntityId == Guid.Empty)
                yield return "EntityId must not be empty.";

            if (string.IsNullOrWhiteSpace(EntityType))
                yield return "EntityType discriminator must not be empty.";

            if (CurrentStepIndex < 0)
                yield return "CurrentStepIndex cannot be negative.";

            if (Steps.Count > 0 && CurrentStepIndex >= Steps.Count)
                yield return "CurrentStepIndex is out of range for the step collection.";
        }

        public override string GetDisplayName() => $"Workflow[{Type}|{Status}]:{Id}";
    }
}
