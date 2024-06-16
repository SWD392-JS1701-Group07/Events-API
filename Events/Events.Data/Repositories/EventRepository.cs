using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
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
        public async Task<bool> Add(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatus(Event eventEntity)
        {
            var local = _context.Set<Event>().Local.FirstOrDefault(entry => entry.Id.Equals(eventEntity.Id));
            if (local != null)
            {
                // Detach the local instance if it exists.
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(eventEntity).Property(e => e.EventStatus).IsModified = true;
            await _context.SaveChangesAsync();
        }
        public async Task<Event> GetEventById(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task UpdateEvent(Event eventEntity)
        {
            var local = _context.Set<Event>().Local.FirstOrDefault(entry => entry.Id.Equals(eventEntity.Id));
            if (local != null)
            {
                // Detach the local instance if it exists
                _context.Entry(local).State = EntityState.Detached;
            }

            // Update the entity
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteEvent(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }   
        }
        public async Task<IEnumerable<Event>> SearchEventsByNameAsync(string eventName)
        {
            return await _context.Events
                                 .Where(e => EF.Functions.Like(e.Name, $"%{eventName}%"))
                                 .ToListAsync();
        }
        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                                 .FirstOrDefaultAsync(e => e.Id == eventId);
        }

    }
}
