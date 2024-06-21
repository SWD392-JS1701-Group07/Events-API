using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<bool> CreateTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
