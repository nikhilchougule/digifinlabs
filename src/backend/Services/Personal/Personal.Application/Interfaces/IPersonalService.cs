using Personal.Application.DTOs;
using Personal.Entities;

namespace Personal.Application.Interfaces
{
    public interface IPersonalService
    {
        Task<PersonalProfileDto> GetOrCreateProfileAsync(Guid userId, CancellationToken ct = default);
        Task<PersonalProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default);

        Task<List<BankAccountDto>> GetBankAccountsAsync(Guid userId, CancellationToken ct = default);
        Task<BankAccountDto> LinkBankAccountAsync(Guid userId, LinkBankAccountDto dto, CancellationToken ct = default);
        Task UnlinkBankAccountAsync(Guid userId, Guid accountId, CancellationToken ct = default);

        Task<FinanceDashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct = default);

        Task<List<CreditProfileSummaryDto>> GetCreditProfilesAsync(Guid userId, CancellationToken ct = default);

        Task<List<LoanOfferDto>> GetLoanOffersAsync(Guid userId, CancellationToken ct = default);
    }
}
