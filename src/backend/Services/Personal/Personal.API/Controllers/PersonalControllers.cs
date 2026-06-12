using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal.Application.DTOs;
using Personal.Application.Interfaces;

namespace Personal.API.Controllers
{
    [ApiController]
    [Route("api/personal/profile")]
    [Authorize]
    public sealed class ProfileController : ControllerBase
    {
        private readonly IPersonalService _personal;
        public ProfileController(IPersonalService personal) => _personal = personal;

        [HttpGet]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var userId = GetUserId();
            return Ok(await _personal.GetOrCreateProfileAsync(userId, ct));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken ct)
        {
            var userId = GetUserId();
            return Ok(await _personal.UpdateProfileAsync(userId, dto, ct));
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("sub claim missing.");
            return Guid.Parse(sub);
        }
    }

    [ApiController]
    [Route("api/personal/bank-accounts")]
    [Authorize]
    public sealed class BankAccountController : ControllerBase
    {
        private readonly IPersonalService _personal;
        public BankAccountController(IPersonalService personal) => _personal = personal;

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken ct)
            => Ok(await _personal.GetBankAccountsAsync(GetUserId(), ct));

        [HttpPost]
        public async Task<IActionResult> Link([FromBody] LinkBankAccountDto dto, CancellationToken ct)
        {
            var account = await _personal.LinkBankAccountAsync(GetUserId(), dto, ct);
            return CreatedAtAction(nameof(List), account);
        }

        [HttpDelete("{accountId:guid}")]
        public async Task<IActionResult> Unlink(Guid accountId, CancellationToken ct)
        {
            await _personal.UnlinkBankAccountAsync(GetUserId(), accountId, ct);
            return NoContent();
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("sub claim missing.");
            return Guid.Parse(sub);
        }
    }

    [ApiController]
    [Route("api/personal")]
    [Authorize]
    public sealed class PersonalDashboardController : ControllerBase
    {
        private readonly IPersonalService _personal;
        public PersonalDashboardController(IPersonalService personal) => _personal = personal;

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard(CancellationToken ct)
            => Ok(await _personal.GetDashboardAsync(GetUserId(), ct));

        [HttpGet("credit-profiles")]
        public async Task<IActionResult> CreditProfiles(CancellationToken ct)
            => Ok(await _personal.GetCreditProfilesAsync(GetUserId(), ct));

        [HttpGet("loan-offers")]
        public async Task<IActionResult> LoanOffers(CancellationToken ct)
            => Ok(await _personal.GetLoanOffersAsync(GetUserId(), ct));

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("sub claim missing.");
            return Guid.Parse(sub);
        }
    }
}
