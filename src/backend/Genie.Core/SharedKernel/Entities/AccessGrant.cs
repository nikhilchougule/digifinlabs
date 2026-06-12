using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Records every consent-linked document share event.
    /// One DocumentVault entry can accumulate multiple AccessGrants over time.
    ///
    /// DESIGN RULES:
    ///   - Revocation sets RevokedAt — the original grant row is NEVER modified or deleted.
    ///   - ConsentId is mandatory — no document share is valid without an AA consent reference.
    ///   - Enforces RBI AA framework + DPDP Act right-to-withdraw-access.
    /// </summary>
    public sealed class AccessGrant : BaseEntity
    {
        public required Guid VaultId { get; init; }

        /// <summary>Polymorphic FK — resolves to User.Id, APIClient.Id, or Tenant.Id.</summary>
        public required Guid GrantedToId { get; init; }

        public required AccessGranteeType GrantedToType { get; init; }

        /// <summary>
        /// Mandatory reference to the ConsentLog entry authorising this share.
        /// RBI AA framework: every data access must be traceable to a valid consent record.
        /// </summary>
        public required Guid ConsentId { get; init; }

        public DateTime GrantedAt { get; init; } = DateTime.UtcNow;

        public required DateTime ExpiresAt { get; init; }

        /// <summary>
        /// Set on revocation. Original grant row is never deleted.
        /// DPDP Act: right-to-withdraw-access must be honoured within 72 hours.
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>True only when the grant is within its validity window and not revoked.</summary>
        public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

        // Navigation
        public DocumentVault? Vault { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (VaultId == Guid.Empty)
                yield return "VaultId must not be empty.";

            if (GrantedToId == Guid.Empty)
                yield return "GrantedToId must not be empty.";

            if (ConsentId == Guid.Empty)
                yield return "Every document share must reference a ConsentLog entry (RBI AA mandate).";

            if (ExpiresAt <= GrantedAt)
                yield return "ExpiresAt must be after GrantedAt.";
        }
    }
}
