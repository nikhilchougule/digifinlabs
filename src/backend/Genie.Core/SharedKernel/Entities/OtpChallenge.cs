using SharedKernel.Common;
using SharedKernel.Enums;
using System;

namespace SharedKernel.Entities
{
    /// <summary>
    /// A short-lived one-time-password challenge.
    /// Used for Mobile OTP login and Email address verification.
    /// </summary>
    public sealed class OtpChallenge : BaseEntity
    {
        public required string Target { get; set; }       // phone number or email address
        public required OtpPurpose Purpose { get; set; }
        public required string CodeHash { get; set; }     // SHA-256 hash of the OTP
        public required DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public int AttemptCount { get; set; } = 0;

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsValid => !IsUsed && !IsExpired && AttemptCount < 5;

        public override string GetDisplayName() => $"OtpChallenge[{Purpose}]:{Target}";
    }
}