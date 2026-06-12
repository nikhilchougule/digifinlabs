namespace SharedKernel.Enums
{
    /// <summary>
    /// Risk band derived from a credit or business score.
    /// Used by both the Business vertical (BusinessCreditScore) and Underwriting (CreditDecision).
    /// Kept in SharedKernel so neither vertical has a cross-dependency on the other.
    /// </summary>
    public enum RiskBand
    {
        VeryLow,
        Low,
        Medium,
        High
    }
}
