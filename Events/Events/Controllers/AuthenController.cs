using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Events.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Events.Data.DTOs.Request;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/auth")]
    [ApiVersionNeutral]
    public class AuthenController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthenController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var user = await _accountService.CheckLogin(loginRequest.Username, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password!");
            }
            else
            {
                string accessToken = JWTGenerator.GenerateToken(user);
                return Ok(new
                {
                    AccountId = user.Id,
                    FullName = user.Name,
                    Role = user.RoleId,
                    accessToken = accessToken
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            JWTGenerator.InvalidateToken(token);
            return Ok(new { message = "Logout successfully" });
        }
    }
}
