using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Immutable record of a user's consent grant.
    /// Inherits BaseEntity for Id/CreatedAt.
    /// RevokedAt is the only mutable field — set once on revocation, never cleared.
    /// Mandated by DPDP Act 2023 and RBI AA framework.
    /// </summary>
    public sealed class ConsentLog : BaseEntity
    {
        public required Guid UserId { get; init; }

        public required ConsentType ConsentType { get; init; }

        /// <summary>UTC time the user explicitly granted consent. May differ from CreatedAt for backdated imports.</summary>
        public required DateTime ConsentedAt { get; init; }

        public required DateTime ExpiryAt { get; init; }

        /// <summary>
        /// Set on revocation. Triggers downstream data suspension per DPDP Act.
        /// Never clear this once set — revocation is permanent.
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>Verbatim purpose string shown to the user at consent time — stored as legal evidence.</summary>
        public required string Purpose { get; init; }

        /// <summary>Id of the party granted access.</summary>
        public required Guid GrantedToId { get; init; }

        /// <summary>Discriminator — identifies whether the grantee is a User, API client, or Tenant.</summary>
        public required AccessGranteeType GrantedToType { get; init; }

        public bool IsActive => RevokedAt is null && ExpiryAt > DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}
