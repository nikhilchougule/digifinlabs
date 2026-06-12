using Los.Application.Enums;
using SharedKernel.Common;
using System;

namespace Los.Application.Events
{
    /// <summary>
    /// Published when a new application is created. Routing key: application.created
    /// Consumers: underwriting (shell), risk, ai, workflow, audit. Kicks off the loan saga.
    /// </summary>
    public sealed class ApplicationCreated : BaseDomainEvent
    {
        public required Guid ApplicationId { get; init; }
        public required ApplicationType ApplicationType { get; init; }
        public required ApplicantType ApplicantType { get; init; }
        public required Guid ApplicantId { get; init; }
    }
}
