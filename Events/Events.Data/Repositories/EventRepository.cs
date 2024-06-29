using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using static Events.Utils.Enums;

namespace Events.Data.Repositories
{
	public class EventRepository : IEventRepository
	{
        private readonly EventsDbContext _context;

        public EventRepository(EventsDbContext eventsDbContext)
        {
            _context = eventsDbContext;
         }
        public async Task<List<Event>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Event> eventQuery = _context.Events;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                eventQuery = eventQuery.Where(p => p.Name.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortOrder))
            {
                Expression<Func<Event, object>> keySelector = sortColumn switch
                {
                    "name" => e => e.Name,
                    "price" => e => e.Price,
                    "quantity" => e => e.Quantity,
                    "startselldate" => e => e.StartSellDate,
                    "endselldate" => e => e.EndSellDate,
                    _ => e => e.Id, // Default sorting
                };

                eventQuery = sortOrder.ToLower() switch
                {
                    "asc" => eventQuery.OrderBy(keySelector),
                    "desc" => eventQuery.OrderByDescending(keySelector),
                    _ => eventQuery.OrderBy(keySelector)
                };
            }
            else
            {
                eventQuery = eventQuery.OrderBy(e => e.Id);
            }

            eventQuery = eventQuery.Skip((page - 1) * pageSize).Take(pageSize);

            var events = await eventQuery.ToListAsync();
            return events;
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

		public async Task<double> GetPriceOfEvent(int eventId)
		{
			var eventFound = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            return eventFound!.Price;
		}

		public async Task<bool> UpdateTicketQuantity(Event eventEntity, int quantity)
		{
            var local = _context.Set<Event>().Local.FirstOrDefault(entity => entity.Id == eventEntity.Id);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
            if(quantity < eventEntity.Remaining)
            {
				eventEntity.Remaining -= quantity;
			}
			_context.Events.Update(eventEntity);
            return await _context.SaveChangesAsync() > 0;
		}
    
            public async Task<List<Event>> GetEventsByStatus(EventStatus status)
        {
            var statusInt = (int)status;
            var events = await _context.Events
                .Where(e => (int)e.EventStatus == statusInt)
                .ToListAsync();

            // Logging the number of events retrieved

            return events;
        }
    }
	}

