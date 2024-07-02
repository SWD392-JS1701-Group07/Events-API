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
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService=orderService;
		}

		[HttpPost("create")]
        public async Task<IActionResult> CreateOrderAndPayment([FromBody] CreateOrderRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
				return BadRequest(new BaseResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					Message = "Invalid request body",
					IsSuccess = false
				});
			}
			var response = await _orderService.CreateOrderAndPayment(request, HttpContext);
			return StatusCode(response.StatusCode, response);
		}

        [HttpGet("/callback")]
        public async Task<IActionResult> Callback()
        {
            var response = await _orderService.HandlePaymentCallback(Request.Query);
            return StatusCode(response.StatusCode, response);
        }
    }
}