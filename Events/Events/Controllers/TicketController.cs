
﻿using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
	[Route("api/tickets")]
	[ApiController]
	[ApiVersion("1.0")]
	[ApiExplorerSettings(GroupName = "v1")]
	[ApiVersionNeutral]
	public class TicketController : ControllerBase
	{
		private readonly ITicketService _ticketService;
		public TicketController(ITicketService ticketService) {
			_ticketService = ticketService;
		}

		[HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTicket([FromQuery] bool? isBought = null,
													[FromQuery] string? searchTerm = null,
													[FromQuery] string? orderId = null,
													[FromQuery] string email = "johnDoe1@gmail.com")
		{
			var response = await _ticketService.GetTicketFilter(email, isBought, orderId: orderId, searchTerm, includeProps: "Orders,Event");
			return StatusCode(response.StatusCode, response);
			//return !ticketDtos.Any()
			//	? NotFound(new BaseResponse
			//	{
			//		StatusCode = StatusCodes.Status404NotFound,
			//		Message = "Not found any Ticket !!!",
			//		IsSuccess = false
			//	})
			//	: Ok(new BaseResponse	
			//	{
			//		StatusCode = StatusCodes.Status200OK,
			//		Data = ticketDtos,
			//		IsSuccess = true
			//	});
		}

		[HttpGet("{id}", Name = nameof(GetTicketById))]
        [Authorize]
        public async Task<IActionResult> GetTicketById([FromRoute] string id)
		{
			var response = await _ticketService.GetTicketById(id);
			return StatusCode(response.StatusCode, response);
		}

        [HttpGet("event/{id}")]
        [Authorize(Roles = "4,5")]
        public async Task<IActionResult> GetTicketByEventId(int id, string? searchTerm)
        {
            var response = await _ticketService.GetTicketByEventId(id, searchTerm);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("event/{id}")]
        [Authorize(Roles = "4,5")]
		public async Task<IActionResult> UpdateTicketStatus(string id, string status)
		{
            var response = await _ticketService.UpdateTicketStatus(id, status);
            return StatusCode(response.StatusCode, response);
        }
    }
}
