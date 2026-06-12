using Genie.Persistence;
using Identity.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;

namespace Identity.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly GenieDbContext _db;

        public UserRepository(GenieDbContext db) => _db = db;

        public Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<User?> FindByProviderKeyAsync(string providerKey, CancellationToken ct = default)
            => _db.Users
                  .AsNoTracking()
                  .Join(_db.Credentials,
                        u => u.Id,
                        c => c.UserId,
                        (u, c) => new { User = u, Cred = c })
                  .Where(x => x.Cred.ProviderKey == providerKey)
                  .Select(x => x.User)
                  .FirstOrDefaultAsync(ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public Task<OtpChallenge?> FindActiveOtpAsync(string target, CancellationToken ct = default)
            => _db.OtpChallenges
                  .AsNoTracking()
                  .Where(x => x.Target == target && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                  .OrderByDescending(x => x.CreatedAt)
                  .FirstOrDefaultAsync(ct);

        public async Task AddOtpChallengeAsync(OtpChallenge challenge, CancellationToken ct = default)
        {
            await _db.OtpChallenges.AddAsync(challenge, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateOtpChallengeAsync(OtpChallenge challenge, CancellationToken ct = default)
        {
            _db.OtpChallenges.Update(challenge);
            await _db.SaveChangesAsync(ct);
        }

        public Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken ct = default)
            => _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _db.RefreshTokens.AddAsync(token, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync(ct);
        }

        public async Task RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = await _db.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync(ct);

            foreach (var t in tokens)
            {
                t.IsRevoked = true;
                t.RevokedAt = DateTime.UtcNow;
                t.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
