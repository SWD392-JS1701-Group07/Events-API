using Events.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/accounts")]
    [ApiVersionNeutral]

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccounts();

            return StatusCode(accounts.StatusCode, accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _accountService.GetAccountById(id);
            
            return StatusCode(account.StatusCode, account);
        }
    }
}
