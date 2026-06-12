using Los.Application.Enums;
using SharedKernel.Common;
using System;
using System.Collections.Generic;

namespace Los.Servicing.Entities
{
    /// <summary>
    /// The repayment schedule generated for an Application at sanction — aggregate root for installments.
    ///
    /// DESIGN RULES:
    ///   - Generated once at sanction; regenerated on restructuring (new schedule, old preserved).
    ///   - Frequency reuses the Application RepaymentFrequency enum.
    /// </summary>
    public sealed class RepaymentSchedule : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required RepaymentFrequency Frequency { get; init; }

        public required int TotalInstallments { get; set; }

        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;

        // Navigation (in-aggregate)
        public List<Repayment> Repayments { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (TotalInstallments <= 0)
                yield return "TotalInstallments must be greater than zero.";
        }

        public override string GetDisplayName() => $"RepaymentSchedule[{TotalInstallments}x{Frequency}]:{Id}";
    }
}
