using Events.Data.DTOs;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);

        Task<bool> Add(Event newEvent);
        Task SaveChangesAsync();
        Task UpdateStatus(Event eventEntity);

        Task<Event> GetEventById(int id);

        Task UpdateEvent(Event eventToUpdate);

        Task DeleteEvent(int id);
        Task<IEnumerable<Event>> SearchEventsByNameAsync(string eventName);
        Task<Event> GetEventByIdAsync(int eventId);
        Task<double> GetPriceOfEvent(int eventId);
        Task<bool> UpdateTicketQuantity(Event eventEntity, int quantity);
    }
}
