namespace Personal.Enums
{
    /// <summary>
    /// Lifecycle state of a LoanOffer presented via the Loan Discovery Engine.
    /// Shown → Applied triggers the hard bureau enquiry (CIBIL hit).
    /// Revenue event: Disbursed triggers the loan facilitation fee.
    /// </summary>
    public enum LoanOfferStatus
    {
        Shown,
        Applied,
        Rejected,
        Disbursed
    }
}
