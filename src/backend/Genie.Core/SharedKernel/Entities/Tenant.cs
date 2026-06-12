using SharedKernel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Entities
{
    /// <summary>
    /// A white-label platform tenant — an NBFC, bank, or fintech partner operating on DigifinLabs infrastructure.
    /// Every User, Application, and data record is scoped to a Tenant for strict data isolation.
    /// </summary>
    public sealed class Tenant : BaseAuditableEntity
    {
        /// <summary>Human-readable name of the tenant organisation.</summary>
        public required string Name { get; set; }

        /// <summary>URL-safe unique identifier, e.g. "acme-nbfc". Used for subdomain routing.</summary>
        public required string Slug { get; set; }

        public Guid PlanId { get; set; }

        /// <summary>Feature flags and per-tenant limits as JSONB. e.g. { maxUsers: 500, aanEnabled: true }</summary>
        public Dictionary<string, object> Settings { get; set; } = new();

        /// <summary>White-label branding config as JSONB. e.g. { logoUrl, primaryColor, domain }</summary>
        public Dictionary<string, object> Branding { get; set; } = new();

        public bool IsActive { get; set; } = true;

        public override string GetDisplayName() => $"Tenant[{Slug}]:{Id}";
    }
}
