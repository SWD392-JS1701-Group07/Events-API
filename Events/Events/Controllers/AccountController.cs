using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
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
      //  [Authorize(Roles ="1")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccounts();

            return StatusCode(accounts.StatusCode, accounts);
        }

        [HttpGet("{id}")]
    //    [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _accountService.GetAccountById(id);
            
            return StatusCode(account.StatusCode, account);
        }

        [HttpGet("role/{id}")]
        //[Authorize(Roles = "1")]
        public async Task<IActionResult> GetAccountByRoleId(int id)
        {
            var account = await _accountService.GetAccountByRole(id);

            return StatusCode(account.StatusCode, account);
        }

        [HttpPost]
      //  [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO account)
        {
            var accountCreate = await _accountService.CreateAccount(account);
            
            return StatusCode(accountCreate.StatusCode, accountCreate);
        }

        /// <summary>
        /// Soft delete account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> BanAccount(int id)
        {
            var account = await _accountService.BanAccount(id);
            return StatusCode(account.StatusCode, account); 
        }

        /// <summary>
        /// Update account for admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDTO accountDTO)
        {
            var account = await _accountService.UpdateAccount(id, accountDTO);
            return StatusCode(account.StatusCode, account);
        }
        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateProfile"></param>
        /// <returns></returns>
        [HttpPut("update-profile/{id}")]
        [Authorize(Roles = "1,2,3,4,5")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfile updateProfile)
        {
            var account = await _accountService.UpdateProfile(id, updateProfile);
            return StatusCode(account.StatusCode, account);
        }
    }
}
