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
using Microsoft.AspNetCore.Http;

namespace Events.Business.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDTO>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<BaseResponse> CreateEvent(CreateEventDTO createEventDTO);
        Task<BaseResponse> GetEventById(int id);
        Task UpdateStatus(int id, EventStatus newStatus);
        Task<BaseResponse> UpdateEventDetails(int id, UpdateEventDTO updateEventDTO);
        Task<List<EventDTO>> GetEventsByStatus(EventStatus status);
        Task DeleteEvent(int id);
        Task<IEnumerable<EventDTO>> SearchEventsByNameAsync(string eventName);
        Task<string> GetEventNameByIdAsync(int eventId);
		Task<double> GetTotalPriceTicketOfEvent(List<TicketDetail> tickets);
		Task<bool> UpdateTicketQuantity(Dictionary<int, int> eventTicketQuantities);
        Task<BaseResponse> GetEventByCollaboratorId(int id);
        Task<BaseResponse> UploadImageForEvent(int id, IFormFile? file);
	}
}
