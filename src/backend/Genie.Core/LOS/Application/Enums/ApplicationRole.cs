namespace Los.Application.Enums
{
    /// <summary>
    /// Staff roles that act on an application — used for assignment, the four-eye rule, and approvals.
    /// (LOS-scoped view of the platform RBAC roles.)
    /// </summary>
    public enum ApplicationRole
    {
        SalesRm,
        CreditAnalyst,
        CreditManager,
        RiskOfficer,
        RiskHead,
        Cro
    }
}
