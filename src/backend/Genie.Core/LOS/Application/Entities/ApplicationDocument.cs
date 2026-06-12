using Los.Application.Enums;
using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace Los.Application.Entities
{
    /// <summary>
    /// A required/collected document for an Application — the per-application checklist line.
    ///
    /// DESIGN RULES:
    ///   - VaultId references document-service (DocumentVault) by UUID once uploaded; no FK.
    ///   - AiExtractedData / AnomalyFlags are populated by ai-service via events.
    ///   - VerificationStatus reuses the SharedKernel DocumentVerificationStatus enum.
    /// </summary>
    public sealed class ApplicationDocument : BaseAuditableEntity
    {
        public required Guid ApplicationId { get; init; }

        public required DocumentType DocType { get; init; }

        public required DocumentCategory Category { get; init; }

        public bool IsMandatory { get; set; } = true;

        /// <summary>DocumentVault.Id once uploaded. Null while still outstanding.</summary>
        public Guid? VaultId { get; set; }

        public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.Pending;

        public UploadStage UploadStage { get; set; } = UploadStage.Sales;

        public Guid? RequestedById { get; set; }
        public Guid? UploadedById { get; set; }   // sales may upload on behalf of the customer
        public Guid? VerifiedById { get; set; }

        /// <summary>OCR / AI extracted fields (JSONB). Null until ai-service processes the doc.</summary>
        public Dictionary<string, object> AiExtractedData { get; set; } = new();

        public Dictionary<string, object> AnomalyFlags { get; set; } = new();

        public DateOnly? ExpiryDate { get; set; }
        public DateTime? UploadedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public bool IsVerified => Status is
            DocumentVerificationStatus.AutoVerified or
            DocumentVerificationStatus.ManuallyVerified;

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (ApplicationId == Guid.Empty)
                yield return "ApplicationId must not be empty.";

            if (Status != DocumentVerificationStatus.Pending && VaultId is null)
                yield return "A non-pending document must reference an uploaded VaultId.";

            if (IsVerified && VerifiedAt is null)
                yield return "VerifiedAt must be set when a document is verified.";
        }

        public override string GetDisplayName() => $"AppDoc[{DocType}|{Status}]:{Id}";
    }
}
