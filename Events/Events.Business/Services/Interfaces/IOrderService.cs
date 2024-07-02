using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
	public interface IOrderService
	{
		Task<BaseResponse> CreateOrderAndPayment(CreateOrderRequest request, HttpContext context);
		Task<BaseResponse> HandlePaymentCallback(IQueryCollection query);
	}
}
