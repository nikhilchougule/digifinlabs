using Personal.Entities;

namespace Personal.Application.Interfaces
{
    public interface IPersonalProfileRepository
    {
        Task<PersonalProfile?> FindByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<PersonalProfile?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(PersonalProfile profile, CancellationToken ct = default);
        Task UpdateAsync(PersonalProfile profile, CancellationToken ct = default);
    }

    public interface IBankAccountRepository
    {
        Task<BankAccount?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<BankAccount>> ListByUserAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(BankAccount account, CancellationToken ct = default);
        Task UpdateAsync(BankAccount account, CancellationToken ct = default);
    }

    public interface ICreditProfileRepository
    {
        Task<CreditProfile?> FindLatestByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<CreditProfile>> ListByUserAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(CreditProfile profile, CancellationToken ct = default);
    }

    public interface ILoanOfferRepository
    {
        Task<List<LoanOffer>> ListActiveByUserAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(LoanOffer offer, CancellationToken ct = default);
        Task UpdateAsync(LoanOffer offer, CancellationToken ct = default);
    }

    public interface IFinanceDashboardRepository
    {
        Task<FinanceDashboard?> FindByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task UpsertAsync(FinanceDashboard dashboard, CancellationToken ct = default);
    }
}
