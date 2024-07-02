
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

		public async Task CreateOrders(Order order)
		{
			await _context.Orders.AddAsync(order);
		}

		public async Task<Order> GetOrderByIdAsync(string id)
		{
			return await _context.Orders.Include(o => o.Tickets).FirstOrDefaultAsync(o => o.Id == id) ?? throw new KeyNotFoundException("Order not found !!");
		}

		public async Task<bool> UpdateOrderStatusAsync(Order order)
		{
			try
			{
				_context.Entry(order).State = EntityState.Modified;
				return await _context.SaveChangesAsync() > 0;
			} catch (Exception)
			{
				throw;
			}
		}
	}
}
