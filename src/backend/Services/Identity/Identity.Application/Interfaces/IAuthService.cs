using Identity.Application.DTOs;
using SharedKernel.Entities;

namespace Identity.Application.Interfaces
{
    /// <summary>
    /// Orchestrates all authentication flows:
    ///   OTP request → OTP verify (create/find user) → token pair
    ///   Token refresh → revoke
    /// This is always called synchronously. Side effects (AuditLog, session tracking)
    /// are published as async events after the main operation completes.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates an OTP for the given mobile/email and stores its hash.
        /// Returns the challenge ID so the client can pass it on verification.
        /// </summary>
        Task<Guid> RequestOtpAsync(OtpRequestDto dto, CancellationToken ct = default);

        /// <summary>
        /// Verifies the OTP. On success, upserts the User record and issues a token pair.
        /// On first login the user account is created automatically (OTP = implicit registration).
        /// </summary>
        Task<AuthResultDto> VerifyOtpAsync(OtpVerifyDto dto, CancellationToken ct = default);

        /// <summary>
        /// Issues a new access token from a valid non-expired refresh token.
        /// The old refresh token is revoked and replaced (rotation strategy).
        /// </summary>
        Task<AuthResultDto> RefreshTokenAsync(TokenRefreshDto dto, CancellationToken ct = default);

        /// <summary>
        /// Revokes the given refresh token. Subsequent refresh attempts with it will fail.
        /// </summary>
        Task RevokeTokenAsync(TokenRevokeDto dto, CancellationToken ct = default);

        /// <summary>
        /// Returns the User for display in /me endpoint, resolving profile refs.
        /// </summary>
        Task<User?> GetUserAsync(Guid userId, CancellationToken ct = default);
    }
}
