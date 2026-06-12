using Personal.Enums;

namespace Personal.Application.DTOs
{
    public sealed record PersonalProfileDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required DateOnly DateOfBirth { get; init; }
        public required char Gender { get; init; }
        public required EmploymentType EmploymentType { get; init; }
        public string? EmployerName { get; init; }
        public string? Designation { get; init; }
    }

    public sealed record UpdateProfileDto
    {
        public string? FullName { get; init; }
        public string? EmployerName { get; init; }
        public string? Designation { get; init; }
        public EmploymentType? EmploymentType { get; init; }
    }

    public sealed record BankAccountDto
    {
        public required Guid Id { get; init; }
        public required string BankName { get; init; }
        public required string Ifsc { get; init; }
        public required string AccountType { get; init; }
        public required bool IsActive { get; init; }
        public required DateTime ConsentExpiry { get; init; }
        public DateTime? LastSyncedAt { get; init; }
    }

    public sealed record LinkBankAccountDto
    {
        public required string EncryptedAccountNumber { get; init; }
        public required int KeyVersion { get; init; }
        public required string Ifsc { get; init; }
        public required string BankName { get; init; }
        public required string AccountType { get; init; }
        public required Guid ConsentId { get; init; }
        public required DateTime ConsentExpiry { get; init; }
    }

    public sealed record FinanceDashboardDto
    {
        public required Guid Id { get; init; }
        public required long TotalAssetsPaise { get; init; }
        public required long TotalLiabilitiesPaise { get; init; }
        public required long NetWorthPaise { get; init; }
        public DateTime? LastRefreshedAt { get; init; }
    }

    public sealed record CreditProfileSummaryDto
    {
        public required Guid Id { get; init; }
        public required string Bureau { get; init; }
        public required DateTime ReportDate { get; init; }
        public required int ActiveLoansCount { get; init; }
        public required int DpdCount { get; init; }
    }

    public sealed record LoanOfferDto
    {
        public required Guid Id { get; init; }
        public required Guid LenderId { get; init; }
        public required string ProductType { get; init; }
        public required long OfferedAmountPaise { get; init; }
        public required decimal InterestRate { get; init; }
        public required int TenureMonths { get; init; }
        public required DateTime OfferExpiry { get; init; }
        public required string Status { get; init; }
    }
}
