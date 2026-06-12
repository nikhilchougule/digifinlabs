using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Security
{
    public sealed class OtpService : IOtpService
    {
        private readonly bool _sandbox;
        private readonly ILogger<OtpService> _logger;

        public OtpService(IConfiguration configuration, ILogger<OtpService> logger)
        {
            _sandbox = string.Equals(configuration["OtpSettings:Sandbox"], "true", StringComparison.OrdinalIgnoreCase);
            _logger = logger;
        }

        public string GenerateCode()
        {
            // Cryptographically random 6-digit code (000000–999999)
            var value = RandomNumberGenerator.GetInt32(0, 1_000_000);
            return value.ToString("D6");
        }

        public string HashCode(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public bool Verify(string code, string storedHash)
        {
            // Constant-time comparison to prevent timing attacks
            var computedHash = HashCode(code);
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHash),
                Encoding.UTF8.GetBytes(storedHash));
        }

        public Task SendAsync(string target, string code, CancellationToken ct = default)
        {
            if (_sandbox)
            {
                // In sandbox the code is visible in the API response — never log in production
                _logger.LogInformation("[SANDBOX] OTP for {Target}: {Code}", target, code);
                return Task.CompletedTask;
            }

            // TODO: Integrate with SMS provider (Exotel/MSG91) and email provider (SendGrid)
            // For now log at warning level so it's visible but clearly flagged as a stub
            _logger.LogWarning("OTP delivery not yet implemented for production. Target: {Target}", target);
            return Task.CompletedTask;
        }
    }
}
