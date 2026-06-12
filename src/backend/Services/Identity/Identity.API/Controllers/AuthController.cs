using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        /// <summary>
        /// Step 1: Request an OTP to the given mobile/email.
        /// Returns a challengeId — pass it to /otp/verify.
        /// In sandbox mode the OTP code is visible in the API response.
        /// </summary>
        [HttpPost("otp/request")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OtpRequestResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDto dto, CancellationToken ct)
        {
            var challengeId = await _auth.RequestOtpAsync(dto, ct);
            return Ok(new OtpRequestResponse(challengeId));
        }

        /// <summary>
        /// Step 2: Verify the OTP. On success returns a JWT access + refresh token pair.
        /// Creates the user account automatically on first login.
        /// </summary>
        [HttpPost("otp/verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _auth.VerifyOtpAsync(dto, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        /// <summary>
        /// Refresh an access token using a valid refresh token (rotation — old token invalidated).
        /// </summary>
        [HttpPost("token/refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _auth.RefreshTokenAsync(dto, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new ProblemDetails { Detail = ex.Message });
            }
        }

        /// <summary>
        /// Revoke a refresh token (logout). Idempotent.
        /// </summary>
        [HttpPost("token/revoke")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRevokeDto dto, CancellationToken ct)
        {
            await _auth.RevokeTokenAsync(dto, ct);
            return NoContent();
        }

        /// <summary>
        /// Returns the authenticated user's profile. Requires a valid JWT bearer token.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _auth.GetUserAsync(userId, ct);
            if (user is null) return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.TenantId,
                Type = user.Type.ToString(),
                KycStatus = user.KycStatus.ToString(),
                user.IsActive
            });
        }

        private sealed record OtpRequestResponse(Guid ChallengeId);
    }
}
