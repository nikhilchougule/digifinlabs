using Los.Servicing.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Servicing.Entities
{
    /// <summary>
    /// A single installment within a RepaymentSchedule.
    ///
    /// DESIGN RULES:
    ///   - DueAmount/PaidAmount use Money. DpdDays (Days Past Due) is the primary delinquency signal —
    ///     it feeds CreditProfile.DpdCount and risk-rule evaluation via a servicing event.
    /// </summary>
    public sealed class Repayment : BaseEntity
    {
        public required Guid ScheduleId { get; init; }

        public required int InstallmentNumber { get; init; }

        public required DateOnly DueDate { get; init; }

        public required Money DueAmount { get; init; }

        public Money? PaidAmount { get; set; }

        public DateTime? PaidAt { get; set; }

        /// <summary>Days past due as of the last servicing run. 0 when current.</summary>
        public int DpdDays { get; set; }

        public RepaymentStatus Status { get; set; } = RepaymentStatus.Scheduled;

        // Navigation (in-aggregate)
        public RepaymentSchedule? Schedule { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ScheduleId == Guid.Empty)
                yield return "ScheduleId must not be empty.";

            if (InstallmentNumber <= 0)
                yield return "InstallmentNumber must be greater than zero.";

            if (DueAmount.AmountInPaise <= 0)
                yield return "DueAmount must be greater than zero.";

            if (DpdDays < 0)
                yield return "DpdDays cannot be negative.";

            if (Status == RepaymentStatus.Paid && PaidAt is null)
                yield return "PaidAt must be set when the installment is Paid.";
        }

        public override string GetDisplayName() => $"Repayment[#{InstallmentNumber}|{Status}]:{Id}";
    }
}
