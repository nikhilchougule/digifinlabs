namespace Business.Enums
{
    /// <summary>
    /// MSME classification per revised RBI/MSME Development Act criteria (June 2020).
    /// Determined by the LOWER of Investment-in-Machinery AND Annual Turnover thresholds.
    /// </summary>
    public enum MsmeClassification
    {
        /// <summary>Investment ≤ ₹1 Cr AND Turnover ≤ ₹5 Cr.</summary>
        Micro,

        /// <summary>Investment ≤ ₹10 Cr AND Turnover ≤ ₹50 Cr.</summary>
        Small,

        /// <summary>Investment ≤ ₹50 Cr AND Turnover ≤ ₹250 Cr.</summary>
        Medium
    }
}
