namespace Business.Enums
{
    /// <summary>GST return form type filed with the GSTN.</summary>
    public enum GstReturnType
    {
        /// <summary>Outward supplies (sales).</summary>
        Gstr1,

        /// <summary>Monthly summary return + tax payment.</summary>
        Gstr3B,

        /// <summary>Annual return.</summary>
        Gstr9
    }
}
