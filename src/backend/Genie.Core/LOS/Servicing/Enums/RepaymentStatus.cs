namespace Los.Servicing.Enums
{
    /// <summary>State of a single scheduled repayment installment.</summary>
    public enum RepaymentStatus
    {
        Scheduled,
        Paid,
        PartiallyPaid,
        Overdue,
        WaivedOff
    }
}
