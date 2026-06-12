using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Unified identity model. Every human or system actor across all verticals is a User.
    /// A single User can operate across multiple verticals via Type.
    /// </summary>
    public sealed class User : BaseAuditableEntity
    {
        public required UserType Type { get; set; }
        public required AuthProvider AuthProvider { get; set; }
        public KycStatus KycStatus { get; set; } = KycStatus.Pending;

        /// <summary>FK → most recently active KycProfile for this user.</summary>
        public Guid? KycRefId { get; set; }

        /// <summary>Polymorphic FK → PersonalProfile or BusinessProfile.</summary>
        public Guid? ProfileRefId { get; set; }

        public required Guid TenantId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; } = false;

        // Navigation
        public Tenant? Tenant { get; set; }
        public KycProfile? KycProfile { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var error in base.Validate())
                yield return error;

            if (TenantId == Guid.Empty)
                yield return "User must belong to a Tenant.";

            if (IsBlocked && IsActive)
                yield return "A blocked user cannot be active simultaneously.";
        }

        public override string GetDisplayName() => $"User[{Type}]:{Id}";
    }
}
