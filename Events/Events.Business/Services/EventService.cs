using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Utils;
using static Events.Utils.Enums;
using System.Diagnostics;
using Events.Data.Repositories;

namespace Events.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventScheduleRepository _eventScheduleRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IEventScheduleRepository eventScheduleRepository, ISponsorRepository sponsorRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _sponsorRepository = sponsorRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDTO>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEvents();

            List<EventDTO> result = new List<EventDTO>();

            foreach(var c in events)
            {
                var example = await _eventScheduleRepository.GetEventScheduleById(c.Id);
                var schedule = _mapper.Map<List<EventScheduleDTO>>(example);

            //    List<EventScheduleDTO> scheduleList = new List<EventScheduleDTO>();

                EventDTO element = new EventDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartSellDate = c.StartSellDate,
                    EndSellDate = c.EndSellDate,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    AvatarUrl = c.AvatarUrl,
                    Description = c.Description,
                    EventStatus = c.EventStatus.ToString(),
                    OwnerId = c.OwnerId,
                    SubjectId = c.SubjectId,
                    ScheduleList = schedule
                };

                result.Add(element);
            }

            return result;
        }

        public async Task<EventDTO> CreateEvent(CreateEventDTO createEventDTO)
        {
            // Check if ScheduleList is not empty
            if (createEventDTO.ScheduleList == null || !createEventDTO.ScheduleList.Any())
            {
                throw new Exception("ScheduleList cannot be empty");
            }

            // Set startDate and endDate based on the first schedule's startTime and endTime
            var firstSchedule = createEventDTO.ScheduleList.First();
            createEventDTO.StartTimeOverall = firstSchedule.StartTime.Date; // Only date part
            createEventDTO.EndTimeOverall = firstSchedule.EndTime.Date; // Only date part
  

            var newEvent = _mapper.Map<Event>(createEventDTO);

            await _eventRepository.Add(newEvent);
            await _eventRepository.SaveChangesAsync(); // Save changes to generate EventId

            foreach (var scheduleDTO in createEventDTO.ScheduleList)
            {
                var newEventSchedule = _mapper.Map<EventSchedule>(scheduleDTO);
                newEventSchedule.EventId = newEvent.Id;
                await _eventScheduleRepository.AddEventScheduleAsync(newEventSchedule);
            }

            return _mapper.Map<EventDTO>(newEvent);
        }
        public async Task<EventDTO> GetEventById(int id)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            return _mapper.Map<EventDTO>(eventEntity);
        }
        public async Task UpdateStatus(int id, EventStatus newStatus)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            if (eventEntity != null)
            {
                eventEntity.EventStatus = newStatus;
                await _eventRepository.UpdateStatus(eventEntity);
            }
        }

        public async Task UpdateEventDetails(int id, CreateEventDTO updateEventDTO)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            // Use AutoMapper or similar tool to map properties from DTO to entity
            _mapper.Map(updateEventDTO, eventEntity);
            eventEntity.Id = id; // Ensure the ID is set correctly

            await _eventRepository.UpdateEvent(eventEntity);
        }

        public async Task<List<EventDTO>> GetEventsByStatus(EventStatus status)
        {
            var events = await _eventRepository.GetAllEvents();
            if (events == null)
            {
                return new List<EventDTO>();
            }

            var filteredEvents = events.Where(e => e.EventStatus == status).ToList();
            return _mapper.Map<List<EventDTO>>(filteredEvents);
        }

        public async Task DeleteEvent(int id)
        {
            var eventToDelete = await _eventRepository.GetEventById(id);
            if (eventToDelete != null)
            {
                await _eventRepository.DeleteEvent(id);
            }
        }
        public async Task<IEnumerable<EventDTO>> SearchEventsByNameAsync(string eventName)
        {
            var events = await _eventRepository.SearchEventsByNameAsync(eventName);
            return _mapper.Map<IEnumerable<EventDTO>>(events);
        }
        public async Task<string> GetEventNameByIdAsync(int eventId)
        {
            var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }
            return eventEntity.Name;
        }
    }
}
