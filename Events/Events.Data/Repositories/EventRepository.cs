using Events.Data.Interfaces;
using Events.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventsDbContext _context;

        public EventRepository(EventsDbContext eventsDbContext)
        {
            _context = eventsDbContext;
        }
        public async Task<List<Event>> GetAllEvents()
        {
            return await _context.Events.ToListAsync();
        }
    }
}
