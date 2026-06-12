using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Enums
{
    internal enum PlatformRole
    {
        // -- CUSTOMER FACING --
        Individual,       // personal vertical user
        BusinessOwner,    // MSME promoter
        Developer,        // API suite consumer

        // -- INTERNAL STAFF (LOS) --
        SalesRM,          // initiates case, collects documents
        CreditAnalyst,    // underwrites, computes ratios
        CreditManager,    // approves up to mid-ticket
        RiskOfficer,      // risk review, evaluates rules
        RiskHead,         // approves high-ticket, overrides soft rules
        CRO,              // Chief Risk Officer — top approval authority

        // -- PLATFORM --
        PlatformAdmin,    // super-admin, all tenants
        TenantAdmin,      // admin for a single tenant
    }
}
