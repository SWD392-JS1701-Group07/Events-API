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
			var existingTicket = await _context.Tickets.Include(t => t.Orders)
			.AnyAsync(t => (t.Email == email
						|| t.PhoneNumber == phoneNumber)
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
										 .FirstOrDefaultAsync(t => t.Id == ticketId) ?? throw new KeyNotFoundException("Ticket not found");
		}

        public async Task<IEnumerable<Ticket>> GetTicketByOrderId(string id)
        {
            return await _context.Tickets.Where(e => e.OrdersId == id).ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketFilter(Account account, int? customerId, bool? isBought = null, string? orderId = null,
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
			if (account.RoleId == 2)
			{
				query = query.Where(t => t.Orders != null && t.Orders.CustomerId.Equals(customerId));
			}
			else if (account.RoleId == 5)
			{
				query = query.Where(t => t.Event != null && t.Event.OwnerId.Equals(account.Id));
			}
			else if(account.RoleId == 1 || account.RoleId == 4) { }
			else
			{
				return new List<Ticket>();
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

        public async Task<bool> UpdateTicket(Ticket ticket)
        {
            try
            {
                _context.Entry(ticket).State = EntityState.Modified;
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
