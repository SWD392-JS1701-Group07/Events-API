
﻿using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Response;
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
		public async Task<IActionResult> GetAllTicket([FromQuery] bool? isBought = null,
													[FromQuery] string? searchTerm = null,
													[FromQuery] string? orderId = null,
													[FromQuery] int accountId = 1)
		{
			var ticketDtos = await _ticketService.GetTicketFilter(accountId, isBought, orderId: orderId, searchTerm, includeProps: "Orders,Event");
			return !ticketDtos.Any()
				? NotFound(new BaseResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					Message = "Not found any Ticket !!!",
					IsSuccess = false
				})
				: Ok(new BaseResponse	
				{
					StatusCode = StatusCodes.Status200OK,
					Data = ticketDtos,
					IsSuccess = true
				});
		}
	}
}
