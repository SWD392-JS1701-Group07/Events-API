
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
		Task<bool> UpdateOrderStatusAsync(Order order, string? responeCode);
    Task<bool> CreateOrders(Order order);
	}
}
