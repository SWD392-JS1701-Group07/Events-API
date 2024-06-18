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
using Events.Models.DTOs.Response;

namespace Events.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventScheduleRepository _eventScheduleRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ISponsorshipRepository _sponsorshipRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepository,
            IEventScheduleRepository eventScheduleRepository,
            ISponsorRepository sponsorRepository,
            ISponsorshipRepository sponsorshipRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _sponsorRepository = sponsorRepository;
            _sponsorshipRepository = sponsorshipRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDTO>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var events = await _eventRepository.GetAllEvents(searchTerm, sortColumn, sortOrder, 1, 10);

            List<EventDTO> result = new List<EventDTO>();

            foreach(var c in events)
            {
                var example = await _eventScheduleRepository.GetEventScheduleById(c.Id);
                var schedule = _mapper.Map<List<EventScheduleDTO>>(example);

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

        public async Task<BaseResponse> CreateEvent(CreateEventDTO createEventDTO)
        {
            // Check if ScheduleList is not empty
            if (createEventDTO.ScheduleList == null || !createEventDTO.ScheduleList.Any())
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "ScheduleList cannot be empty",
                    IsSuccess = false
                };
            }

            // Validate Sponsorships and Sponsors
            List<Sponsorship> sponsorships = new List<Sponsorship>();
            foreach (var sponsorshipDTO in createEventDTO.Sponsorships)
            {
                var sponsorDTO = sponsorshipDTO.Sponsor;
                var existingSponsor = await _sponsorRepository.GetSponsorByEmailAsync(sponsorDTO.Email);

                if (existingSponsor == null)
                {
                    // Check if the account already exists by email
                    var existingAccount = await _accountRepository.GetAccountByEmail(sponsorDTO.Email);
                    if (existingAccount == null)
                    {
                        // Creating new account for sponsor
                        var newAccount = new Account
                        {
                            Name = sponsorDTO.Email.Split('@')[0],
                            Email = sponsorDTO.Email,
                            Username = sponsorDTO.Email,
                            Password = "1",
                            PhoneNumber = sponsorDTO.PhoneNumber,
                            Dob = DateOnly.FromDateTime(DateTime.Now),
                            Gender = Gender.Others,
                            AvatarUrl = null,
                            RoleId = 3,
                            SubjectId = null
                        };
                        await _accountRepository.CreateAccount(newAccount);
                        sponsorDTO.AccountId = newAccount.Id;
                    }
                    else
                    {
                        sponsorDTO.AccountId = existingAccount.Id;
                    }

                    var newSponsor = _mapper.Map<Sponsor>(sponsorDTO);
                    newSponsor.AccountId = sponsorDTO.AccountId;

                    await _sponsorRepository.AddSponsorAsync(newSponsor);
                    await _sponsorRepository.SaveChangesAsync();

                    existingSponsor = newSponsor;
                }

                // Create the sponsorship with the existing or new sponsor ID, and default EventId
                var sponsorship = new Sponsorship
                {
                    EventId = 0, // Temporary default EventId
                    SponsorId = existingSponsor.Id,
                    Description = sponsorshipDTO.Description,
                    Type = sponsorshipDTO.Type,
                    Title = sponsorshipDTO.Title,
                    Sum = sponsorshipDTO.Sum
                };
                sponsorships.Add(sponsorship);
            }

            // Map the DTO to the Event entity
            var newEvent = _mapper.Map<Event>(createEventDTO);

            // Add the event to the repository and save changes to generate EventId
            await _eventRepository.Add(newEvent);
            Console.WriteLine($"Event created with ID: {newEvent.Id}");

            // Delete sponsors with null AccountId and duplicate
            await _sponsorRepository.DeleteDuplicateSponsorsAsync(); ;
            Console.WriteLine("Sponsors with null AccountId deleted.");

            // Add each schedule in the ScheduleList
            foreach (var scheduleDTO in createEventDTO.ScheduleList)
            {
                var newEventSchedule = _mapper.Map<EventSchedule>(scheduleDTO);
                newEventSchedule.EventId = newEvent.Id;
                await _eventScheduleRepository.AddEventScheduleAsync(newEventSchedule);
                Console.WriteLine($"Schedule added with Start Time: {scheduleDTO.StartTime}, End Time: {scheduleDTO.EndTime}, Place: {scheduleDTO.Place}");
            }

            // Update sponsorships with the new EventId
            foreach (var sponsorship in sponsorships)
            {
                sponsorship.EventId = newEvent.Id;
                await _sponsorshipRepository.UpdateSponsorship(sponsorship);
                Console.WriteLine($"Sponsorship updated with new Event ID: {newEvent.Id}");
            }

            var eventDTO = _mapper.Map<EventDTO>(newEvent);

            Console.WriteLine("Event creation process completed. Detailed check:");
            Console.WriteLine($"Event ID: {newEvent.Id}");
            Console.WriteLine($"Schedules count: {createEventDTO.ScheduleList.Count}");
            Console.WriteLine($"Sponsorships count: {createEventDTO.Sponsorships?.Count ?? 0}");

            if (createEventDTO.Sponsorships != null)
            {
                foreach (var sponsorshipDTO in createEventDTO.Sponsorships)
                {
                    Console.WriteLine($"Sponsorship Title: {sponsorshipDTO.Title}");
                    Console.WriteLine($"Sponsor Email: {sponsorshipDTO.Sponsor?.Email}");
                }
            }

            return new BaseResponse
            {
                StatusCode = 201,
                IsSuccess = true,
                Data = eventDTO
            };
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
            var events = await _eventRepository.GetAllEvents(null, null, null, 0, 0);
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
