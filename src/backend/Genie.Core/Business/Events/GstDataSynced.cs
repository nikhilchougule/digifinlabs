using SharedKernel.Common;
using System;

namespace Business.Events
{
    /// <summary>
    /// Published when a GST sync completes for a business.
    /// Routing key: business.gst.synced
    /// Consumers: gst-service, scoring-service, audit-service.
    /// </summary>
    public sealed class GstDataSynced : BaseDomainEvent
    {
        public required Guid BusinessId { get; init; }

        /// <summary>Latest synced period in MM-YYYY format.</summary>
        public required string Period { get; init; }

        /// <summary>Taxable turnover for the period, in paise.</summary>
        public long TurnoverInPaise { get; init; }
    }
}
