using Events.Data.DTOs;
using Events.Data.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Events.Data.Enums;

namespace Events.Business.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDTO>> GetAllEvents();
        Task<EventDTO> CreateEvent(CreateEventDTO createEventDTO);
        Task<EventDTO> GetEventById(int id);
        Task UpdateStatus(int id, EventStatus newStatus);
        Task UpdateEventDetails(EventDTO updateEventDTO);
        Task<List<EventDTO>> GetEventsByStatus(EventStatus status);
        Task DeleteEvent(int id);
    }
}
