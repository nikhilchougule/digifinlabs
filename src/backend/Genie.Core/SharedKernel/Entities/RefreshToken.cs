using SharedKernel.Common;
using System;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Persisted refresh token, scoped to a User + device.
    /// Allows silent re-authentication without re-login.
    /// </summary>
    public sealed class RefreshToken : BaseEntity
    {
        public required Guid UserId { get; set; }
        public required string TokenHash { get; set; }   // SHA-256 of the actual token
        public required string DeviceFingerprint { get; set; }
        public required DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Navigation
        public User? User { get; set; }

        public override string GetDisplayName() => $"RefreshToken:{UserId}";
    }
}