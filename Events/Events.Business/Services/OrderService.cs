using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<bool> UpdateOrderStatus(string orderId, string? responeCode)
		{
			try
			{
				var orderFromDb = await _orderRepository.GetOrderByIdAsync(orderId);
				return await _orderRepository.UpdateOrderStatusAsync(orderFromDb, responeCode);
			}
			catch (Exception) {
				throw;
			}
		}
	}
}
