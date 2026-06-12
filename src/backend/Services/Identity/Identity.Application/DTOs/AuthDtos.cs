using SharedKernel.Enums;

namespace Identity.Application.DTOs
{
    public sealed record OtpRequestDto
    {
        /// <summary>Mobile number (E.164 format) or email address.</summary>
        public required string Target { get; init; }
        public required OtpPurpose Purpose { get; init; }
        public Guid? TenantId { get; init; }
    }

    public sealed record OtpVerifyDto
    {
        public required Guid ChallengeId { get; init; }
        public required string Target { get; init; }
        public required string Code { get; init; }
        /// <summary>Used for refresh token binding — prevents stolen token attacks.</summary>
        public required string DeviceFingerprint { get; init; }
    }

    public sealed record TokenRefreshDto
    {
        public required string RefreshToken { get; init; }
        public required string DeviceFingerprint { get; init; }
    }

    public sealed record TokenRevokeDto
    {
        public required string RefreshToken { get; init; }
    }

    public sealed record AuthResultDto
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
        public required DateTime AccessTokenExpiry { get; init; }
        public required DateTime RefreshTokenExpiry { get; init; }
        public required Guid UserId { get; init; }
        public required string UserType { get; init; }
        /// <summary>Only set in sandbox — not sent in production.</summary>
        public string? SandboxOtpCode { get; init; }
    }
}
