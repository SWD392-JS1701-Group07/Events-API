using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using Events.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Events.Utils.Enums;

namespace Events.Data.Repositories
{
	public class TicketRepository : ITicketRepository
	{
		private readonly EventsDbContext _context;

		public TicketRepository(EventsDbContext context)
		{
			_context = context;
		}

		public async Task<bool> CheckTicketExist(string email, string phoneNumber, int eventId)
		{
			var existingTicket = await _context.Tickets
			.AnyAsync(t => t.Email == email
						&& t.PhoneNumber == phoneNumber
						&& t.EventId == eventId
						&& t.Orders.OrderStatus == Enums.OrderStatus.Success);

			return existingTicket;
		}

		public async Task CreateTicket(Ticket ticket)
		{
			await _context.Tickets.AddAsync(ticket);
		}

		public async Task<Ticket> GetTicketById(string ticketId)
		{
			return await _context.Tickets.Include(t => t.Event)
											.ThenInclude(e => e.EventSchedules)
										 .Include(t => t.Orders).FirstOrDefaultAsync(t => t.Id == ticketId) ?? throw new KeyNotFoundException("Ticket not found");
		}

		public async Task<IEnumerable<Ticket>> GetTicketFilter(Account account, bool? isBought = null, string? orderId = null,
																string? searchTern = null, string? includeProps = null)
		{
			IQueryable<Ticket> query = _context.Tickets;
			if (includeProps != null)
			{
				foreach (var includePro in includeProps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includePro);
				}
			}
			var roleName = account.Role.Name;
			if (string.Equals(roleName, "Visistor", StringComparison.OrdinalIgnoreCase))
			{
				query = query.Where(t => t.Orders != null && t.Orders.CustomerId.Equals(account.Id));
			}
			else if (string.Equals(roleName, "Event operator", StringComparison.OrdinalIgnoreCase))
			{
				query = query.Where(t => t.Event != null && t.Event.OwnerId.Equals(account.Id));
			}
			if (!string.IsNullOrEmpty(searchTern))
			{
				query = query.Where(t =>
					EF.Functions.Like(t.PhoneNumber, $"%{searchTern}%") ||
					EF.Functions.Like(t.Email, $"%{searchTern}%") ||
					EF.Functions.Like(t.Name, $"%{searchTern}%") ||
					EF.Functions.Like(t.Event.Name, $"%{searchTern}%")
				);
			}
			if (isBought.HasValue)
			{
				if (isBought.Value)
				{
					if (!string.IsNullOrEmpty(orderId))
						query = query.Where(t => t.Orders != null && t.Orders.OrderStatus == OrderStatus.Success && t.OrdersId == orderId);
					else
						query = query.Where(t => t.Orders != null && t.Orders.OrderStatus == OrderStatus.Success);
				}
				else
				{
					if (!string.IsNullOrEmpty(orderId))
						query = query.Where(t => t.Orders != null && t.Orders.OrderStatus != OrderStatus.Success && t.OrdersId == orderId);
					else
						query = query.Where(t => t.Orders != null && t.Orders.OrderStatus != OrderStatus.Success);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(orderId))
					query = query.Where(t => t.OrdersId == orderId);
			}
			return await query.ToListAsync();
		}
	}
}
