using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Central encrypted document repository — shared across Personal, Business, and LOS verticals.
    ///
    /// DESIGN RULES:
    ///   - OwnerId is polymorphic: User.Id for individuals, BusinessProfile.Id for MSMEs.
    ///   - StorageKey holds the Azure Blob / S3 object key — NOT a full URL.
    ///     Format: {tenantId}/{ownerId}/{docType}/{guid}.{ext}
    ///     Pre-signed, time-limited URLs (15 min TTL) must be generated at read time only.
    ///   - Access to documents is always mediated through AccessGrant — no direct URL exposure.
    /// </summary>
    public sealed class DocumentVault : BaseAuditableEntity
    {
        /// <summary>Polymorphic owner: User.Id for individuals, BusinessProfile.Id for MSMEs.</summary>
        public required Guid OwnerId { get; init; }

        /// <summary>Discriminator: 'User' | 'BusinessProfile'.</summary>
        public required string OwnerType { get; init; }

        public required DocumentType DocType { get; init; }

        /// <summary>
        /// Azure Blob / S3 object key — NOT a public URL.
        /// Pre-signed URLs with short TTL generated at read time only. Never stored.
        /// </summary>
        public required string StorageKey { get; init; }

        public DocumentVerificationStatus VerificationStatus { get; set; } = DocumentVerificationStatus.Pending;

        public bool IsVerified => VerificationStatus is
            DocumentVerificationStatus.AutoVerified or
            DocumentVerificationStatus.ManuallyVerified;

        public DocumentVerifier? VerifiedByMethod { get; set; }

        /// <summary>FK → User who performed manual verification. Null for system-verified docs.</summary>
        public Guid? VerifiedByUserId { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime UploadedAt { get; init; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }

        /// <summary>All consent-linked shares of this document.</summary>
        public List<AccessGrant> SharedWith { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (OwnerId == Guid.Empty)
                yield return "OwnerId must not be empty.";

            if (string.IsNullOrWhiteSpace(OwnerType))
                yield return "OwnerType discriminator must not be empty.";

            if (string.IsNullOrWhiteSpace(StorageKey))
                yield return "StorageKey must not be empty.";

            if (VerificationStatus != DocumentVerificationStatus.Pending && VerifiedAt is null)
                yield return "VerifiedAt must be set when document has been verified or rejected.";

            if (ExpiryDate.HasValue && ExpiryDate < UploadedAt)
                yield return "ExpiryDate cannot be before UploadedAt.";
        }

        public override string GetDisplayName() => $"Doc[{DocType}]:{Id}";
    }
}
