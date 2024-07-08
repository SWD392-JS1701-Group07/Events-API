
﻿using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Events.Utils.Enums;

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

		public async Task<IEnumerable<Order>> GetAllOrdersFitlter(Account account, int? customerId, bool? isBought = null, string? searchTerm = null, string? includeProps = null)
		{
			IQueryable<Order> query = _context.Orders;
			if (includeProps != null)
			{
				foreach (var includePro in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includePro);
				}
			}
			if (account.RoleId == 2)
			{
				query = query.Where(o => o.CustomerId == customerId);
			}
			if (!string.IsNullOrEmpty(searchTerm))
			{
				query = query.Where(o =>
					EF.Functions.Like(o.PhoneNumber, $"%{searchTerm}%") ||
					EF.Functions.Like(o.Email, $"%{searchTerm}%")
				);
			}
			if (isBought.HasValue)
			{
				if (isBought.Value)
				{
						query = query.Where(o => o.OrderStatus == OrderStatus.Success);
				}
				else
				{
						query = query.Where(o => o.OrderStatus != OrderStatus.Success);
				}
			}
			return await query.ToListAsync();
		}


		public async Task<Order> GetOrderByIdAsync(string id)
		{
			return await _context.Orders.Include(o => o.Tickets)
										.Include(o => o.Transactions)
										.Include(o => o.Customer)
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
