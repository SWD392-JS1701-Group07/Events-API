using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
	public class TicketRepository : ITicketRepository
	{
        private readonly EventsDbContext _context;

        public TicketRepository(EventsDbContext context)
        {
            _context = context;
        }

		public async Task<string> CreateTicket(CreateTicketRequest request)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var orderId = Guid.NewGuid();
					// Tạo order mới
					var order = new Order
					{
						Id = orderId.ToString(),
						OrderDate = DateOnly.FromDateTime(DateTime.Now.Date),
						TotalPrice = request.TotalAmount,
						Notes = request.OrderNotes,
						OrderStatus = request.TotalAmount == 0 ? 1 : 0,
						Email = request.Email,
						PhoneNumber = request.PhoneNumber,
						CustomerId = request.CustomerId
					};

					_context.Orders.Add(order);
					await _context.SaveChangesAsync();

					// Tạo các ticket mới liên kết với order
					foreach (var ticketDetail in request.Tickets)
					{
						var ticket = new Ticket
						{
							Id = Guid.NewGuid().ToString(),
							Name = ticketDetail.Name,
							PhoneNumber = ticketDetail.PhoneNumber,
							Email = ticketDetail.Email,
							EventId = ticketDetail.EventId,
							Price = request.TotalAmount/request.Tickets.Count,
							IsCheckIn = 0,
							OrdersId = orderId.ToString()
						};

						_context.Tickets.Add(ticket);
					}

					await _context.SaveChangesAsync();

					await transaction.CommitAsync();

					return orderId.ToString();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
		}

		public async Task<IEnumerable<Ticket>> GetTicketFilter(bool isBought = false, string? orderId = null, string? includeProps = null)
		{
			IQueryable<Ticket> query = _context.Tickets;
			if (includeProps != null)
			{
				foreach (var includePro in includeProps.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includePro);
				}
			}
			if (isBought)
			{
				if(!string.IsNullOrEmpty(orderId))
				{
					query = query.Where(t => t.Orders != null && t.Orders.OrderStatus == 1 && t.OrdersId == orderId);
				}
				else
				{
					query = query.Where(t => t.Orders != null && t.Orders.OrderStatus == 1);
				}
			}
			else if(!orderId.IsNullOrEmpty())
			{
				query = query.Where(t => t.OrdersId == orderId);
			}
			return await query.ToListAsync();
		}

		public async Task UpdateOrderStatus(string orderId, string? responeCode)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id.Equals(orderId));

					if (order == null)
					{
						throw new Exception("Order not found");
					}

					order.OrderStatus = !responeCode.Equals("00") ? 0 : 1;
					if(responeCode != null)
					{
						order.VnPayResponseCode = responeCode;
					}
					_context.Orders.Update(order);
					await _context.SaveChangesAsync();

					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
		}
	}
}
