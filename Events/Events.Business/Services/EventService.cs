using AutoMapper;
using Events.Business.Interfaces;
using Events.Data.DTOs;
using Events.Data.Interfaces;
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
    }
}
