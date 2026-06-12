namespace Identity.Application.Interfaces
{
    public interface IOtpService
    {
        /// <summary>
        /// Generates a cryptographically random 6-digit OTP code.
        /// </summary>
        string GenerateCode();

        /// <summary>
        /// Returns the SHA-256 hash of the given OTP code for safe storage.
        /// </summary>
        string HashCode(string code);

        /// <summary>
        /// Constant-time comparison of the given code against the stored hash.
        /// </summary>
        bool Verify(string code, string storedHash);

        /// <summary>
        /// Delivers the OTP to the user via the appropriate channel (SMS/email).
        /// In sandbox mode this is a no-op — the code is returned in the API response.
        /// </summary>
        Task SendAsync(string target, string code, CancellationToken ct = default);
    }
}
