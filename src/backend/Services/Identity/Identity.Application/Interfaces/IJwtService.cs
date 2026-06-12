using SharedKernel.Entities;

namespace Identity.Application.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Creates a short-lived signed JWT access token for the given user.
        /// </summary>
        string GenerateAccessToken(User user, out string jti);

        /// <summary>
        /// Creates a cryptographically random opaque refresh token.
        /// Returns the raw token (to be returned to client) and its SHA-256 hash (to be stored).
        /// </summary>
        (string rawToken, string tokenHash) GenerateRefreshToken();

        /// <summary>
        /// Returns the ClaimsPrincipal from an expired access token (used for refresh validation).
        /// Returns null if the token signature/structure is invalid.
        /// </summary>
        System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
