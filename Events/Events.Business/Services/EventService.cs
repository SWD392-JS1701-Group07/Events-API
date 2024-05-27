using AutoMapper;
using Events.Business.Interfaces;
using Events.Data.DTOs;
using Events.Data.Interfaces;
using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
           _eventRepository = eventRepository;
            _mapper = mapper;
        }
        public async Task<List<EventDTO>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEvents();
            var listEventDTO = _mapper.Map<List<EventDTO>>(events);
            return listEventDTO;
        }
        public async Task<EventDTO> CreateEvent(CreateEventDTO createEventDTO)
        {
            var newEvent = _mapper.Map<Event>(createEventDTO);
            await _eventRepository.Add(newEvent);
            await _eventRepository.SaveChangesAsync();
            return _mapper.Map<EventDTO>(newEvent);
        }
        public async Task<EventDTO> GetEventById(int id)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            return _mapper.Map<EventDTO>(eventEntity);
        }

        public async Task UpdateEvent(EventDTO eventDTO)
        {
            var eventEntity = _mapper.Map<Event>(eventDTO);
            _eventRepository.UpdateStatus(eventEntity);
            await _eventRepository.SaveChangesAsync();
        }
        public async Task<List<EventDTO>> GetEventsNeedingApproval()
        {
            var events = await _eventRepository.GetAllEvents();
            if (events == null)
            {
                return new List<EventDTO>();
            }

            var eventsNeedingApproval = events.Where(e => e.EventStatus == 0).ToList();
            return _mapper.Map<List<EventDTO>>(eventsNeedingApproval);
        }
    }
}
