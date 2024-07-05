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
using Events.Utils.Helper;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Events.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventScheduleRepository _eventScheduleRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ISponsorshipRepository _sponsorshipRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICollaboratorRepository _collaboratorRepository;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepository,
            IEventScheduleRepository eventScheduleRepository,
            ISponsorRepository sponsorRepository,
            ISponsorshipRepository sponsorshipRepository,
            IAccountRepository accountRepository,
            ISubjectRepository subjectRepository,
            ICollaboratorRepository collaboratorRepository,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _sponsorRepository = sponsorRepository;
            _sponsorshipRepository = sponsorshipRepository;
            _accountRepository = accountRepository;
            _subjectRepository = subjectRepository;
            _collaboratorRepository = collaboratorRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDTO>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var events = await _eventRepository.GetAllEvents(searchTerm, sortColumn, sortOrder, 1, 10);

            List<EventDTO> result = new List<EventDTO>();

            foreach (var c in events)
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

            // Validation: StartSellDate < EndSellDate
            if (createEventDTO.StartSellDate >= createEventDTO.EndSellDate)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "StartSellDate must be before EndSellDate",
                    IsSuccess = false
                };
            }

            // Map the DTO to the Event entity
            var newEvent = _mapper.Map<Event>(createEventDTO);
            newEvent.Remaining = createEventDTO.Quantity;

            // Check for overlapping events at the same place and time
            foreach (var scheduleDTO in createEventDTO.ScheduleList)
            {
                // Validation: EndSellDate < StartTime
                if (createEventDTO.EndSellDate >= scheduleDTO.StartTime)
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Message = "EndSellDate must be before StartTime",
                        IsSuccess = false
                    };
                }

                // Validation: StartTime < EndTime
                if (scheduleDTO.StartTime >= scheduleDTO.EndTime)
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Message = "StartTime must be before EndTime",
                        IsSuccess = false
                    };
                }

                // Check if there are overlapping events at the same place and time
                var overlappingEvents = await _eventScheduleRepository.GetOverlappingSchedulesAsync(
                    scheduleDTO.Place,
                    scheduleDTO.StartTime,
                    scheduleDTO.EndTime
                );

                if (overlappingEvents.Any())
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Message = "There is an overlapping event at the same place and time",
                        IsSuccess = false
                    };
                }
            }

            // Add the event to the repository and save changes to generate EventId
            await _eventRepository.Add(newEvent);

            foreach (var scheduleDTO in createEventDTO.ScheduleList)
            {
                var newEventSchedule = _mapper.Map<EventSchedule>(scheduleDTO);
                newEventSchedule.EventId = newEvent.Id;
                await _eventScheduleRepository.AddEventScheduleAsync(newEventSchedule);
            }

            await _sponsorRepository.DeleteDuplicateSponsorsAsync();
            await _sponsorRepository.DeleteSponsorsWithNullAccountIdAsync();

            if (createEventDTO.Sponsorships != null)
            {
                // Validate Sponsorships and Sponsors as pairs
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

                        existingSponsor = newSponsor;
                    }

                    // Create the sponsorship with the existing or new sponsor ID, and the new EventId
                    var sponsorship = new Sponsorship
                    {
                        EventId = newEvent.Id,
                        SponsorId = existingSponsor.Id,
                        Description = sponsorshipDTO.Description,
                        Type = sponsorshipDTO.Type,
                        Title = sponsorshipDTO.Title,
                        Sum = sponsorshipDTO.Sum
                    };

                    // Add the sponsorship to the repository
                    await _sponsorshipRepository.CreateSponsorship(sponsorship);
                }
            }

            var eventDTO = _mapper.Map<EventDTO>(newEvent);

            return new BaseResponse
            {
                StatusCode = 201,
                IsSuccess = true,
                Data = eventDTO
            };
        }



        public async Task<BaseResponse> GetEventById(int id)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Data = null,
                    Message = "Event not found"
                };
            }
            else
            {
                var scheduleEntity = await _eventScheduleRepository.GetEventScheduleById(id);
                var scheduleDto = _mapper.Map<List<EventScheduleDTO>>(scheduleEntity);

                var sponsorships = await _sponsorshipRepository.GetAllSponsorshipsByEventId(id);
                var sponsorshipsDTO = _mapper.Map<List<SponsorshipDTO>>(sponsorships);

                var owner = await _accountRepository.GetAccountById(eventEntity.OwnerId);
                var ownerDTO = _mapper.Map<AccountDTO>(owner);

                var subject = await _subjectRepository.GetSubjectById((int)eventEntity.SubjectId);
                var subjectDTO = _mapper.Map<SubjectDTO>(subject);


                EventDetailsResponseDTO eventDetails = new EventDetailsResponseDTO
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    StartSellDate = eventEntity.StartSellDate,
                    EndSellDate = eventEntity.EndSellDate,
                    Price = eventEntity.Price,
                    Quantity = eventEntity.Quantity,
                    Remaining = eventEntity.Remaining,
                    AvatarUrl = eventEntity.AvatarUrl,
                    Description = eventEntity.Description,
                    EventStatus = eventEntity.EventStatus.ToString(),
                    OwnerId = eventEntity.OwnerId,
                    SubjectId = eventEntity.SubjectId,
                    Subject = subjectDTO,
                    EventOperator = ownerDTO,
                    ScheduleList = scheduleDto,
                    Sponsorships = sponsorshipsDTO,
                };

                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = eventDetails,
                    Message = null
                };
            }

            //  return eventDto;
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
            var events = await _eventRepository.GetEventsByStatus(status);
            if (events == null || events.Count == 0)
            {
                return new List<EventDTO>();
            }

            var eventDTOs = _mapper.Map<List<EventDTO>>(events);

            foreach (var eventDTO in eventDTOs)
            {
                var schedules = await _eventScheduleRepository.GetEventScheduleById(eventDTO.Id);
                eventDTO.ScheduleList = _mapper.Map<List<EventScheduleDTO>>(schedules);
            }

            return eventDTOs;
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

        public async Task<double> GetTotalPriceTicketOfEvent(List<TicketDetail> tickets)
        {
            double total = 0;
            foreach (var ticketDetail in tickets)
            {
                total += await _eventRepository.GetPriceOfEvent(ticketDetail.EventId);
            }
            return total;
        }

        public async Task<bool> UpdateTicketQuantity(Dictionary<int, int> eventTicketQuantities)
        {
			foreach (var entry in eventTicketQuantities)
			{
				var eventId = entry.Key;
				var ticketCount = entry.Value;

                var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
				if (eventEntity == null)
				{
					throw new KeyNotFoundException("Event not found");
				}

				bool isSuccess = await _eventRepository.UpdateTicketQuantity(eventEntity, ticketCount);
                if (!isSuccess)
                {
                    return false;
                }
			}
            return true;
		}

        public async Task<BaseResponse> GetEventByCollaboratorId(int id)
        {
            var collaboratorId = await _accountRepository.GetAccountById(id);
            if (collaboratorId == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Data = null,
                    Message = "Can't found this collaborators"
                };
            }
            else
            {
                //                var collaborator = _mapper.Map<AccountDTO>(collaboratorId);
                List<EventDTO> eventList = new List<EventDTO>();
                var eventIdList = await _collaboratorRepository.GetAllEventIdByCollaboratorId(id);
                foreach(var eventId in eventIdList)
                {
                    var eventEntity = await _eventRepository.GetEventByIdAsync(eventId);
                    var eventSchedule = await _eventScheduleRepository.GetEventScheduleById(eventId);
                    EventDTO eventMapper = new EventDTO
                    {
                        Id = eventId,
                        Name = eventEntity.Name,
                        StartSellDate = eventEntity.StartSellDate,
                        EndSellDate = eventEntity.EndSellDate,
                        Price = eventEntity.Price,
                        Quantity = eventEntity.Quantity,
                        Remaining = eventEntity.Remaining,
                        AvatarUrl = eventEntity.AvatarUrl,
                        Description = eventEntity.Description,
                        EventStatus = eventEntity.EventStatus.ToString(),
                        OwnerId = eventEntity.OwnerId,
                        SubjectId = eventEntity.SubjectId,
                        ScheduleList = _mapper.Map<List<EventScheduleDTO>>(eventSchedule)
                    };
                    eventList.Add(eventMapper);
                }

                List<EventWithCollaborators> eventWithCollaboratorList = new List<EventWithCollaborators>();

                foreach (var c in eventList)
                {
                    var collaborator = await _collaboratorRepository.GetCollaboratorByEventAndAccount(c.Id, id);
                    EventWithCollaborators entity = new EventWithCollaborators
                    {
                        eventDTO = c,
                        collaboratorDTO = _mapper.Map<CollaboratorDTO>(collaborator)
                    };
                    eventWithCollaboratorList.Add(entity);
                }

                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = eventWithCollaboratorList,
                    Message = null
                };
            }
        }
    }
}
