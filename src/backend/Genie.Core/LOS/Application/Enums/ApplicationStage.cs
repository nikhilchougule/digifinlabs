namespace Los.Application.Enums
{
    /// <summary>
    /// Lifecycle stage of an Application. Drives the TAT clock and the Stage → Owner → SLA matrix.
    /// TAT starts at DocumentCollection (first document uploaded) and ends at the risk decision.
    /// </summary>
    public enum ApplicationStage
    {
        Lead,
        DocumentCollection,
        CreditReview,
        RiskReview,
        Sanctioned,
        Rejected,
        Disbursed,
        Closed,
        OnHold
    }
}
