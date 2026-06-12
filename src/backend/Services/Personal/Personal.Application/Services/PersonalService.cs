using Genie.Persistence;
using Microsoft.EntityFrameworkCore;
using Personal.Application.DTOs;
using Personal.Application.Interfaces;
using Personal.Entities;
using SharedKernel.ValueObjects;

namespace Personal.Application.Services
{
    public sealed class PersonalService : IPersonalService
    {
        private readonly GenieDbContext _db;

        public PersonalService(GenieDbContext db) => _db = db;

        public async Task<PersonalProfileDto> GetOrCreateProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var profile = await _db.PersonalProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (profile is not null)
                return ToDto(profile);

            // Stub profile — user will fill details during onboarding
            var newProfile = new PersonalProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = string.Empty,
                DateOfBirth = DateOnly.MinValue,
                Gender = 'U',
                EmploymentType = Personal.Enums.EmploymentType.Salaried
            };

            await _db.PersonalProfiles.AddAsync(newProfile, ct);
            await _db.SaveChangesAsync(ct);

            return ToDto(newProfile);
        }

        public async Task<PersonalProfileDto> UpdateProfileAsync(
            Guid userId, UpdateProfileDto dto, CancellationToken ct = default)
        {
            var profile = await _db.PersonalProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId, ct)
                ?? throw new InvalidOperationException("Personal profile not found.");

            if (dto.FullName is not null) profile.FullName = dto.FullName;
            if (dto.EmployerName is not null) profile.EmployerName = dto.EmployerName;
            if (dto.Designation is not null) profile.Designation = dto.Designation;
            if (dto.EmploymentType is not null) profile.EmploymentType = dto.EmploymentType.Value;

            profile.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            return ToDto(profile);
        }

        public async Task<List<BankAccountDto>> GetBankAccountsAsync(Guid userId, CancellationToken ct = default)
        {
            var accounts = await _db.BankAccounts
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync(ct);

            return accounts.Select(a => new BankAccountDto
            {
                Id = a.Id,
                BankName = a.BankName,
                Ifsc = a.Ifsc,
                AccountType = a.AccountType,
                IsActive = a.IsActive,
                ConsentExpiry = a.ConsentExpiry,
                LastSyncedAt = a.LastSyncedAt
            }).ToList();
        }

        public async Task<BankAccountDto> LinkBankAccountAsync(
            Guid userId, LinkBankAccountDto dto, CancellationToken ct = default)
        {
            var account = new BankAccount
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountNumber = new EncryptedValue(dto.EncryptedAccountNumber, dto.KeyVersion),
                Ifsc = dto.Ifsc,
                BankName = dto.BankName,
                AccountType = dto.AccountType,
                ConsentId = dto.ConsentId,
                ConsentExpiry = dto.ConsentExpiry,
                IsActive = true
            };

            await _db.BankAccounts.AddAsync(account, ct);
            await _db.SaveChangesAsync(ct);

            return new BankAccountDto
            {
                Id = account.Id,
                BankName = account.BankName,
                Ifsc = account.Ifsc,
                AccountType = account.AccountType,
                IsActive = account.IsActive,
                ConsentExpiry = account.ConsentExpiry
            };
        }

        public async Task UnlinkBankAccountAsync(Guid userId, Guid accountId, CancellationToken ct = default)
        {
            var account = await _db.BankAccounts
                .FirstOrDefaultAsync(x => x.Id == accountId && x.UserId == userId, ct)
                ?? throw new InvalidOperationException("Bank account not found.");

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task<FinanceDashboardDto> GetDashboardAsync(Guid userId, CancellationToken ct = default)
        {
            var dashboard = await _db.FinanceDashboards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (dashboard is null)
            {
                dashboard = new FinanceDashboard
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TotalAssets = new SharedKernel.ValueObjects.Money(0),
                    TotalLiabilities = new SharedKernel.ValueObjects.Money(0),
                    NetWorth = new SharedKernel.ValueObjects.Money(0)
                };
                await _db.FinanceDashboards.AddAsync(dashboard, ct);
                await _db.SaveChangesAsync(ct);
            }

            return new FinanceDashboardDto
            {
                Id = dashboard.Id,
                TotalAssetsPaise = dashboard.TotalAssets.AmountInPaise,
                TotalLiabilitiesPaise = dashboard.TotalLiabilities.AmountInPaise,
                NetWorthPaise = dashboard.NetWorth.AmountInPaise,
                LastRefreshedAt = dashboard.LastRefreshedAt
            };
        }

        public async Task<List<CreditProfileSummaryDto>> GetCreditProfilesAsync(
            Guid userId, CancellationToken ct = default)
        {
            var profiles = await _db.CreditProfiles
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ReportDate)
                .ToListAsync(ct);

            return profiles.Select(p => new CreditProfileSummaryDto
            {
                Id = p.Id,
                Bureau = p.Bureau.ToString(),
                ReportDate = p.ReportDate,
                ActiveLoansCount = p.ActiveLoansCount,
                DpdCount = p.DpdCount
            }).ToList();
        }

        public async Task<List<LoanOfferDto>> GetLoanOffersAsync(Guid userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var offers = await _db.LoanOffers
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.OfferExpiry > now)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return offers.Select(o => new LoanOfferDto
            {
                Id = o.Id,
                LenderId = o.LenderId,
                ProductType = o.ProductType.ToString(),
                OfferedAmountPaise = o.OfferedAmount.AmountInPaise,
                InterestRate = o.InterestRate,
                TenureMonths = o.TenureMonths,
                OfferExpiry = o.OfferExpiry,
                Status = o.Status.ToString()
            }).ToList();
        }

        private static PersonalProfileDto ToDto(PersonalProfile p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            FullName = p.FullName,
            DateOfBirth = p.DateOfBirth,
            Gender = p.Gender,
            EmploymentType = p.EmploymentType,
            EmployerName = p.EmployerName,
            Designation = p.Designation
        };
    }
}
