using Events.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDTO>> GetAllEvents();
        Task<EventDTO> CreateEvent(CreateEventDTO createEventDTO);
        Task<EventDTO> GetEventById(int id);
        Task UpdateEvent(EventDTO eventDTO);
        Task<List<EventDTO>> GetEventsNeedingApproval();
    }
}
