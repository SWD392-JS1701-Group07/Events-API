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
    public class EventScheduleRepository : IEventScheduleRepository
    {
        private readonly EventsDbContext _context;

        public EventScheduleRepository(EventsDbContext context)
        {
            _context = context;
        }
        public async Task<List<EventSchedule>> GetAllEventSchedule()
        {
            return await _context.EventSchedules.ToListAsync();  
        }

        public async Task<List<EventSchedule>> GetEventScheduleById(int id)
        {
            return await _context.EventSchedules.Where(e => e.EventId == id).ToListAsync();
        }
        public async Task<bool> AddEventScheduleAsync(EventSchedule eventSchedule)
        {
            await _context.EventSchedules.AddAsync(eventSchedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<EventSchedule>> GetOverlappingSchedulesAsync(string place, DateTime startTime, DateTime endTime)
        {
            return await _context.EventSchedules
                .Where(es => es.Place == place &&
                             ((es.StartTime <= startTime && es.EndTime >= startTime) ||
                              (es.StartTime <= endTime && es.EndTime >= endTime) ||
                              (es.StartTime >= startTime && es.EndTime <= endTime)))
                .ToListAsync();
        }
        public async Task<List<EventSchedule>> GetOverlappingSchedulesPlace(DateTime startTime, DateTime endTime)
        {
            return await _context.EventSchedules
                     .Where(es =>
                            (es.StartTime <= startTime && es.EndTime >= startTime) ||
                            (es.StartTime <= endTime && es.EndTime >= endTime) ||
                            (es.StartTime >= startTime && es.EndTime <= endTime))
                     .ToListAsync();
        }
    }
}
