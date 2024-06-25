using System;
using System.Linq;
using System.Threading.Tasks;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/event/order")]
    [ApiVersionNeutral]
    public class OrderController : ControllerBase
    {
        private readonly ITicketService _ticketService;
		private readonly IEventService _eventService;
		private readonly IVNPayPaymentService _vnPayPaymentService;

        public OrderController(ITicketService ticketService, IEventService eventService, IVNPayPaymentService vnPayPaymentService)
        {
            _ticketService = ticketService;

            _vnPayPaymentService = vnPayPaymentService;
            _eventService = eventService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrderAndPayment([FromBody] CreateTicketRequest request)
        {
            var orderId = "";
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request body");
            }
            try
            {
				var totalPriceFromDb = await _eventService.GetTotalPriceTicketOfEvent(request.Tickets);
                if(request.TotalAmount == totalPriceFromDb)
                {
                    orderId = await _ticketService.CreateTicket(request);
					if (request.TotalAmount == 0)
					{
						return Ok(new { messsage = "Buy ticket successfully", success = true });
					}
				}
                else
                {
                    return BadRequest("Total Amount is not correct!!!");
                }
                var amount = request.TotalAmount.ToString();
                var orderInfo = $"Thanh toan don hang #{orderId}";
                var payUrl = "";
                if (request.PaymentType == "VNPAY")
                {
                    payUrl = _vnPayPaymentService.CreatePayment(amount, orderId.ToString(), orderInfo, HttpContext);
                }

                return Ok(new { payUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var response = _vnPayPaymentService.PaymentExecute(Request.Query);
			if (response.Success)
            {
				string orderId = "";
				string[] parts = response.OrderDescription.Split('#');
				if (parts.Length > 1)
				{
					orderId = parts[1].Trim();
				}
				await _ticketService.UpdateOrderStatus(orderId, response.VnPayResponseCode);
                if(response.VnPayResponseCode.Equals("00"))
                {
                    var ticketBought = await _ticketService.GetTicketFilter(true,orderId, includeProps: "Orders");
                    if(ticketBought.Any())
                    {
						var ticketBoughtCount = ticketBought.Count();
                        var ticketEventId = ticketBought.First().EventId;
						bool isSuccess =  await _eventService.UpdateTicketQuantity(ticketEventId, ticketBoughtCount);
                        if(!isSuccess)
                        {
                            return BadRequest(new BaseResponse
                            {
                                Message = "Have an error when update ticket quantity!!",
                                StatusCode = 500,
                                IsSuccess = isSuccess
                            });
                        }
                    }
                }
			}
            return Ok(new { response });
        }
    }
}