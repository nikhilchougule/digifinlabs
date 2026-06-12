using Los.Application.Enums;
using SharedKernel.Common;
using System;

namespace Los.Application.Events
{
    /// <summary>
    /// Published when a document is uploaded against an application. Routing key: application.document.uploaded
    /// Consumers: ai (extraction), checklist, audit. Also starts the TAT clock on the first upload.
    /// </summary>
    public sealed class ApplicationDocumentUploaded : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public required Guid DocumentId { get; init; }
        public required Guid VaultId { get; init; }
        public UploadStage UploadStage { get; init; }
    }
}
