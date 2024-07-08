
﻿using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
	public interface IOrderRepository
	{
		Task<Order> GetOrderByIdAsync(string id);
		Task<bool> UpdateOrderStatusAsync(Order order);
		Task CreateOrders(Order order);

		Task<IEnumerable<Order>> GetAllOrdersFitlter(Account account, int? customerId, bool? isBought = null, string? searchTerm = null, string? includeProps = null);
	}
}
