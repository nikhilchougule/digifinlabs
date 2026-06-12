using SharedKernel.Common;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Stores one authentication method per User per AuthProvider.
    /// A User may have multiple Credentials (e.g., Email + Google + MobileOtp).
    /// </summary>
    public sealed class Credential : BaseAuditableEntity
    {
        public required Guid UserId { get; set; }
        public required AuthProvider Provider { get; set; }

        /// <summary>
        /// The unique identifier from the provider:
        /// - Email     → email address
        /// - Google    → Google subject ID
        /// - Microsoft → Microsoft object ID
        /// - Facebook  → Facebook user ID
        /// - MobileOtp → E.164 phone number e.g. +919876543210
        /// </summary>
        public required string ProviderKey { get; set; }

        /// <summary>Bcrypt hash — only populated for Email/Password provider.</summary>
        public string? PasswordHash { get; set; }

        public bool IsVerified { get; set; } = false;

        // Navigation
        public User? User { get; set; }

        public override IEnumerable<string> Validate()
        {
            foreach (var error in base.Validate())
                yield return error;

            if (UserId == Guid.Empty)
                yield return "Credential must be linked to a User.";

            if (string.IsNullOrWhiteSpace(ProviderKey))
                yield return "ProviderKey is required.";

            if (Provider == AuthProvider.Email && string.IsNullOrWhiteSpace(PasswordHash))
                yield return "Email provider requires a password hash.";
        }

        public override string GetDisplayName() => $"Credential[{Provider}]:{ProviderKey}";
    }
}