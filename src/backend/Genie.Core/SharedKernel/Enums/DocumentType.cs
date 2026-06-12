namespace SharedKernel.Enums
{
    /// <summary>
    /// Canonical document type taxonomy shared across Personal, Business, and LOS verticals.
    /// </summary>
    public enum DocumentType
    {
        // Identity
        Aadhaar,
        Pan,
        Passport,
        VoterId,
        DrivingLicence,

        // Income (Personal)
        Itr,
        Payslip,
        Form16,

        // Banking
        BankStatement,

        // Business
        GstCertificate,
        GstReturn,
        CaCertificate,
        BalanceSheet,
        ProfitAndLoss,
        Udyam,
        BusinessRegistration,

        // Collateral / Legal (LOS)
        PropertyDocs,
        CollateralDocs,
        LegalOpinion,
        ValuationReport,

        // Loan Lifecycle (LOS)
        SanctionLetter,
        LoanAgreement,
        DisbursementAdvice,

        // Other
        FacePhoto,
        Other
    }
}
