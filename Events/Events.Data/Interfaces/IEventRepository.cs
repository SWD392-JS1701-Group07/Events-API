using Events.Data.DTOs;
using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEvents();

        Task Add(Event newEvent);
        Task SaveChangesAsync();
        Task UpdateStatus(Event eventEntity);

        Task<Event> GetEventById(int id);

        Task UpdateEvent(Event eventToUpdate);

        Task DeleteEvent(int id);
    }
}
