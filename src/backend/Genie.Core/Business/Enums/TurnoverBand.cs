namespace Business.Enums
{
    /// <summary>
    /// Annual turnover band for an MSME. Bands aligned to the MSME (Udyam) classification
    /// thresholds and common underwriting buckets. Stored rather than the raw figure to keep
    /// turnover out of query projections.
    /// NOTE: exact cut-offs to be confirmed with Credit/Risk before persistence wiring.
    /// </summary>
    public enum TurnoverBand
    {
        /// <summary>Up to ₹40 lakh.</summary>
        UpTo40Lakh,

        /// <summary>₹40 lakh – ₹1.5 crore.</summary>
        From40LakhTo1_5Crore,

        /// <summary>₹1.5 crore – ₹5 crore (Micro upper / Small lower).</summary>
        From1_5CroreTo5Crore,

        /// <summary>₹5 crore – ₹50 crore (Small / Medium).</summary>
        From5CroreTo50Crore,

        /// <summary>₹50 crore – ₹250 crore (Medium).</summary>
        From50CroreTo250Crore,

        /// <summary>Above ₹250 crore.</summary>
        Above250Crore
    }
}
