using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/ticket")]
    [ApiVersionNeutral]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        /// <summary>
        /// Join a free event
        /// </summary>
        /// <param name="createTicketRequestDTO"></param>
        /// <returns></returns>
        [HttpPost("free")]
        public async Task<IActionResult> CreateFreeTicket([FromBody] CreateTicketRequestDTO createTicketRequestDTO)
        {
            var ticketCreate = await _ticketService.CreateFreeTicket(createTicketRequestDTO);

            return StatusCode(ticketCreate.StatusCode, ticketCreate);
        }
    }
}
