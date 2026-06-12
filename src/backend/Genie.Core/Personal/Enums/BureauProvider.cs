namespace Personal.Enums
{
    /// <summary>
    /// Credit bureau from which a CreditProfile was pulled.
    /// Bureau data access is regulated under CICRA — every pull requires
    /// explicit borrower consent and a ConsentLog entry.
    /// </summary>
    public enum BureauProvider
    {
        Cibil,
        Experian,
        Equifax,
        Crif
    }
}
