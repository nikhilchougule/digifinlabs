using Los.Servicing.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Los.Servicing.Entities
{
    /// <summary>
    /// A single disbursement tranche / revolving draw against a sanctioned Application.
    ///
    /// DESIGN RULES:
    ///   - 1..N per Application (term loans usually one; working capital draws many).
    ///   - Amount uses Money. BankRefNumber is the external payout reference.
    /// </summary>
    public sealed class Disbursement : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required int TrancheNumber { get; init; }

        public required Money Amount { get; set; }

        public DisbursementStatus Status { get; set; } = DisbursementStatus.Pending;

        public DateTime? DisbursedAt { get; set; }

        public string? BankRefNumber { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (TrancheNumber <= 0)
                yield return "TrancheNumber must be greater than zero.";

            if (Amount.AmountInPaise <= 0)
                yield return "Disbursement Amount must be greater than zero.";

            if (Status == DisbursementStatus.Disbursed && DisbursedAt is null)
                yield return "DisbursedAt must be set once the tranche is Disbursed.";
        }

        public override string GetDisplayName() => $"Disbursement[#{TrancheNumber}|{Status}]:{Id}";
    }
}
