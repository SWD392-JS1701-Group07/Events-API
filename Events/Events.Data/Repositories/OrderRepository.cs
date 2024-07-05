
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

		//public Task<IEnumerable<Order>> GetAllOrdersFitlter(Account account, bool? isBought = null, string? searchTerm = null)
		//{
		//	IQueryable<Order> query = _context.Orders.Include(o => o.Customer);
		//	var roleName = account.Role.Name;
		//	if (string.Equals(roleName, "Visistor", StringComparison.OrdinalIgnoreCase))
		//	{
		//		query = query.Where(o => o.Customer != null && o.Customer.Id == account.Id);
		//	}
		//	else if (string.Equals(roleName, "Event operator", StringComparison.OrdinalIgnoreCase))
		//	{
		//		query = query.Where(t => t.Event != null && t.Event.OwnerId.Equals(account.Id));
		//	}
		//}

		public async Task<Order> GetOrderByIdAsync(string id)
		{
			return await _context.Orders.Include(o => o.Tickets)
											.Include(o => o.Transactions)
											.FirstOrDefaultAsync(o => o.Id == id)
											?? throw new KeyNotFoundException("Order not found !!");
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
