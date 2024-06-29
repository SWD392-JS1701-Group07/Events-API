
﻿using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly EventsDbContext _context;
        public OrderRepository(EventsDbContext context)
        {
            _context = context;
        }

		public async Task<Order> GetOrderByIdAsync(string id)
		{
			return await _context.Orders.FindAsync(id) ?? throw new KeyNotFoundException("Order not found !!");
		}

		public async Task<bool> UpdateOrderStatusAsync(Order order, string? responeCode)
		{
			try
			{
				if (responeCode != null)
				{
					order.OrderStatus = responeCode.Equals("00") ? 1 : 0;
					order.VnPayResponseCode = responeCode;
				}
				_context.Entry(order).State = EntityState.Modified;
				return await _context.SaveChangesAsync() > 0;
			} catch (Exception)
			{
				throw;
			}
		}
	}
}
