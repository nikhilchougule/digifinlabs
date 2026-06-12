using Genie.Persistence;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedKernel.Entities;
using SharedKernel.Enums;

namespace Identity.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private const int OtpExpiryMinutes = 10;
        private const int MaxOtpAttempts = 5;

        private readonly IUserRepository _users;
        private readonly IJwtService _jwt;
        private readonly IOtpService _otp;
        private readonly GenieDbContext _db;
        private readonly bool _isSandbox;

        public AuthService(
            IUserRepository users,
            IJwtService jwt,
            IOtpService otp,
            GenieDbContext db,
            IConfiguration configuration)
        {
            _users = users;
            _jwt = jwt;
            _otp = otp;
            _db = db;
            _isSandbox = string.Equals(configuration["OtpSettings:Sandbox"], "true", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Guid> RequestOtpAsync(OtpRequestDto dto, CancellationToken ct = default)
        {
            // Invalidate any previous active challenges for this target + purpose
            var existing = await _db.OtpChallenges
                .Where(x => x.Target == dto.Target && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(ct);

            foreach (var old in existing)
            {
                old.IsUsed = true;
                old.UpdatedAt = DateTime.UtcNow;
                _db.OtpChallenges.Update(old);
            }

            var code = _otp.GenerateCode();
            var codeHash = _otp.HashCode(code);

            var challenge = new OtpChallenge
            {
                Id = Guid.NewGuid(),
                Target = dto.Target,
                Purpose = dto.Purpose,
                CodeHash = codeHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
                IsUsed = false,
                AttemptCount = 0
            };

            await _users.AddOtpChallengeAsync(challenge, ct);

            if (!_isSandbox)
                await _otp.SendAsync(dto.Target, code, ct);

            return challenge.Id;
        }

        public async Task<AuthResultDto> VerifyOtpAsync(OtpVerifyDto dto, CancellationToken ct = default)
        {
            var challenge = await _db.OtpChallenges
                .FirstOrDefaultAsync(x => x.Id == dto.ChallengeId && x.Target == dto.Target, ct)
                ?? throw new InvalidOperationException("OTP challenge not found.");

            if (challenge.IsUsed)
                throw new InvalidOperationException("OTP has already been used.");

            if (challenge.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("OTP has expired.");

            challenge.AttemptCount++;
            challenge.UpdatedAt = DateTime.UtcNow;

            if (challenge.AttemptCount > MaxOtpAttempts)
            {
                challenge.IsUsed = true;
                await _users.UpdateOtpChallengeAsync(challenge, ct);
                throw new InvalidOperationException("Maximum OTP attempts exceeded. Please request a new code.");
            }

            if (!_otp.Verify(dto.Code, challenge.CodeHash))
            {
                await _users.UpdateOtpChallengeAsync(challenge, ct);
                throw new InvalidOperationException("Invalid OTP code.");
            }

            challenge.IsUsed = true;
            await _users.UpdateOtpChallengeAsync(challenge, ct);

            // Upsert user — first login creates the account automatically
            var user = await _users.FindByProviderKeyAsync(dto.Target, ct);
            if (user is null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    TenantId = Guid.Empty, // Will be assigned by onboarding flow
                    Type = UserType.Individual,
                    AuthProvider = AuthProvider.MobileOtp,
                    KycStatus = KycStatus.Pending,
                    IsActive = true
                };
                await _users.AddAsync(user, ct);

                var credential = new Credential
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Provider = AuthProvider.MobileOtp,
                    ProviderKey = dto.Target,
                    IsVerified = true
                };
                await _db.Credentials.AddAsync(credential, ct);
                await _db.SaveChangesAsync(ct);
            }

            return await IssueTokenPairAsync(user, dto.DeviceFingerprint, ct);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(TokenRefreshDto dto, CancellationToken ct = default)
        {
            var tokenHash = ComputeSha256(dto.RefreshToken);
            var stored = await _users.FindRefreshTokenByHashAsync(tokenHash, ct)
                ?? throw new InvalidOperationException("Refresh token not found.");

            if (stored.IsRevoked)
                throw new InvalidOperationException("Refresh token has been revoked.");

            if (stored.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Refresh token has expired.");

            if (stored.DeviceFingerprint != dto.DeviceFingerprint)
                throw new InvalidOperationException("Device fingerprint mismatch.");

            var user = await _users.FindByIdAsync(stored.UserId, ct)
                ?? throw new InvalidOperationException("User not found.");

            // Rotate: revoke old token
            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;
            stored.UpdatedAt = DateTime.UtcNow;
            await _users.UpdateRefreshTokenAsync(stored, ct);

            return await IssueTokenPairAsync(user, dto.DeviceFingerprint, ct);
        }

        public async Task RevokeTokenAsync(TokenRevokeDto dto, CancellationToken ct = default)
        {
            var tokenHash = ComputeSha256(dto.RefreshToken);
            var stored = await _users.FindRefreshTokenByHashAsync(tokenHash, ct);
            if (stored is null) return; // Idempotent

            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;
            stored.UpdatedAt = DateTime.UtcNow;
            await _users.UpdateRefreshTokenAsync(stored, ct);
        }

        public Task<User?> GetUserAsync(Guid userId, CancellationToken ct = default)
            => _users.FindByIdAsync(userId, ct);

        // ── private helpers ───────────────────────────────────────────────────

        private async Task<AuthResultDto> IssueTokenPairAsync(
            User user, string deviceFingerprint, CancellationToken ct)
        {
            var accessToken = _jwt.GenerateAccessToken(user, out _);
            var (rawRefresh, refreshHash) = _jwt.GenerateRefreshToken();

            var accessExpiry = DateTime.UtcNow.AddHours(1);
            var refreshExpiry = DateTime.UtcNow.AddDays(30);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshHash,
                DeviceFingerprint = deviceFingerprint,
                ExpiresAt = refreshExpiry,
                IsRevoked = false
            };

            await _users.AddRefreshTokenAsync(refreshToken, ct);

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefresh,
                AccessTokenExpiry = accessExpiry,
                RefreshTokenExpiry = refreshExpiry,
                UserId = user.Id,
                UserType = user.Type.ToString()
            };
        }

        private static string ComputeSha256(string input)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
