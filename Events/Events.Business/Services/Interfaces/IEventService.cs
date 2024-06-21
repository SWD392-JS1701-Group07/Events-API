using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Utils;
using static Events.Utils.Enums;
using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Hosting;

namespace Events.Business.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDTO>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<BaseResponse> CreateEvent(CreateEventDTO createEventDTO);
        Task<EventDTO> GetEventById(int id);
        Task UpdateStatus(int id, EventStatus newStatus);
        Task UpdateEventDetails(int id, CreateEventDTO updateEventDTO);
        Task<List<EventDTO>> GetEventsByStatus(EventStatus status);
        Task DeleteEvent(int id);
        Task<IEnumerable<EventDTO>> SearchEventsByNameAsync(string eventName);
        Task<string> GetEventNameByIdAsync(int eventId);
    }
}
