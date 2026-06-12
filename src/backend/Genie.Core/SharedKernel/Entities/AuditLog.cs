using SharedKernel.Common;
using SharedKernel.Enums;
using SharedKernel.Interfaces;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Immutable record of every data mutation on a regulated entity.
    /// Inherits BaseEntity for Id/CreatedAt. Implements IAppendOnly — no row
    /// may ever be updated or deleted (enforced by EF Core interceptor + DB trigger).
    /// </summary>
    public sealed class AuditLog : BaseEntity, IAppendOnly
    {
        /// <summary>Name of the entity type being audited, e.g. "BankAccount", "LoanOffer", "Application".</summary>
        public required string EntityType { get; init; }

        public required Guid EntityId { get; init; }

        public required AuditAction Action { get; init; }

        /// <summary>UserId of the actor. Use a sentinel "system" Guid for background jobs.</summary>
        public required Guid PerformedBy { get; init; }

        public IpAddress? IpAddress { get; init; }

        public string? UserAgent { get; init; }

        /// <summary>JSON diff of changed fields: { fieldName: { from: oldValue, to: newValue } }</summary>
        public Dictionary<string, object> Diff { get; init; } = new();
    }
}
