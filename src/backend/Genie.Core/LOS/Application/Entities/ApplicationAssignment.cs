using Los.Application.Enums;
using SharedKernel.Common;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// Assignment of a staff member to an Application in a specific role, with handover history.
    ///
    /// DESIGN RULES:
    ///   - AssignedToId / AssignedById reference identity-service by UUID — no FK.
    ///   - Supports the four-eye rule: no single user may hold Sales, Credit, and Risk on one application
    ///     (enforced on Application; assignments record the audit of who held what, when).
    /// </summary>
    public sealed class ApplicationAssignment : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required Guid AssignedToId { get; init; }

        public required ApplicationRole Role { get; init; }

        public Guid? AssignedById { get; set; }

        public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

        public DateTime? AcceptedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public string? HandoverNotes { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (AssignedToId == Guid.Empty)
                yield return "AssignedToId must not be empty.";
        }

        public override string GetDisplayName() => $"Assignment[{Role}]:{ApplicationId}";
    }
}
