namespace Business.Enums
{
    /// <summary>Filing state of a single GST return period.</summary>
    public enum GstFilingStatus
    {
        /// <summary>Filed on or before the due date.</summary>
        Filed,

        /// <summary>Filed after the due date — contributes to DelayDays and lowers filing-consistency score.</summary>
        FiledLate,

        /// <summary>Period is due but not yet filed (within or beyond the window).</summary>
        Pending,

        /// <summary>Period was never filed — strongest negative filing signal.</summary>
        NotFiled
    }
}
