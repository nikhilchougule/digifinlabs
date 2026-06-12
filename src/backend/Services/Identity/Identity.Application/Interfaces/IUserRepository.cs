using SharedKernel.Entities;

namespace Identity.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> FindByProviderKeyAsync(string providerKey, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);

        Task<OtpChallenge?> FindActiveOtpAsync(string target, CancellationToken ct = default);
        Task AddOtpChallengeAsync(OtpChallenge challenge, CancellationToken ct = default);
        Task UpdateOtpChallengeAsync(OtpChallenge challenge, CancellationToken ct = default);

        Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken ct = default);
        Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
        Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
        Task RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken ct = default);
    }
}
