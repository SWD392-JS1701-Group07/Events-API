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
using Events.Utils.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

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
        private readonly CloudinaryHelper _cloudinaryHelper;
        private readonly EmailHelper _emailHelper;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepository,
            IEventScheduleRepository eventScheduleRepository,
            ISponsorRepository sponsorRepository,
            ISponsorshipRepository sponsorshipRepository,
            IAccountRepository accountRepository,
            ISubjectRepository subjectRepository,
            ICollaboratorRepository collaboratorRepository,
            CloudinaryHelper cloudinaryHelper,
            EmailHelper emailHelper,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _sponsorRepository = sponsorRepository;
            _sponsorshipRepository = sponsorshipRepository;
            _accountRepository = accountRepository;
            _subjectRepository = subjectRepository;
            _collaboratorRepository = collaboratorRepository;
            _cloudinaryHelper = cloudinaryHelper;
            _emailHelper = emailHelper;
            _mapper = mapper;
        }

        public async Task<List<EventDTO>> GetAllEvents(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var events = await _eventRepository.GetAllEvents(searchTerm, sortColumn, sortOrder, page, pageSize);

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
            try
            {
                // Validate DTO for whitespace-only fields
                ValidationUtils.ValidateNoWhitespaceOnly(createEventDTO);
            }
            catch (ArgumentException ex)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
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
            if (createEventDTO.StartSellDate < DateTime.Now)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "StartSellDate must be after the current time",
                    IsSuccess = false
                };
            }

            if (createEventDTO.EndSellDate <= DateTime.Now)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "EndSellDate must be after the current time",
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

            if(createEventDTO.SubjectId == 0)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "SubjectId can not be null",
                    IsSuccess = false
                };
            }

            // Check for duplicate schedules within the list
            var scheduleTimes = new HashSet<(string Place, DateTime StartTime, DateTime EndTime)>();
            foreach (var schedule in createEventDTO.ScheduleList)
            {
                if (!scheduleTimes.Add((schedule.Place, schedule.StartTime, schedule.EndTime)))
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Message = "Duplicate schedules are not allowed",
                        IsSuccess = false
                    };
                }
            }

            // Check for overlapping schedules within the user's input list
            for (int i = 0; i < createEventDTO.ScheduleList.Count; i++)
            {
                for (int j = i + 1; j < createEventDTO.ScheduleList.Count; j++)
                {
                    var schedule1 = createEventDTO.ScheduleList[i];
                    var schedule2 = createEventDTO.ScheduleList[j];
                    if (schedule1.Place == schedule2.Place &&
                        schedule1.StartTime < schedule2.EndTime &&
                        schedule1.EndTime > schedule2.StartTime)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 400,
                            Message = "Schedules within the list overlap",
                            IsSuccess = false
                        };
                    }
                }
            }

            // Check for overlapping events at the same place and time in the database
            foreach (var scheduleDTO in createEventDTO.ScheduleList)
            {
                // Validation: EndSellDate < StartTime
                if (createEventDTO.EndSellDate > scheduleDTO.StartTime)
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

            //if(createEventDTO.Sponsorships != null)
            //{
            //    foreach (var sponsorshipDTO in createEventDTO.Sponsorships)
            //    {
            //        var sponsorDTO = sponsorshipDTO.Sponsor;
            //        if (sponsorDTO != null)
            //        {
            //            // Validate email format
            //            if (!ValidationUtils.IsValidEmail(sponsorDTO.Email))
            //            {
            //                return new BaseResponse
            //                {
            //                    StatusCode = 400,
            //                    Message = "Invalid email format",
            //                    IsSuccess = false
            //                };
            //            }

            //            // Validate phone number format
            //            if (!ValidationUtils.IsPhoneNumber(sponsorDTO.PhoneNumber))
            //            {
            //                return new BaseResponse
            //                {
            //                    StatusCode = 400,
            //                    Message = "Phone number should contain only digits",
            //                    IsSuccess = false
            //                };
            //            }

            //            var existEmailSponsors = await _sponsorRepository.GetSponsorByEmailAsync(sponsorDTO.Email);
            //            var existPhoneSponsors = await _sponsorRepository.GetSponsorByPhoneNumberAsync(sponsorDTO.PhoneNumber);

            //            if (existPhoneSponsors == null && existEmailSponsors == null)
            //            {
            //                var existingAccountsByEmail = await _accountRepository.GetAccountsByEmailList(sponsorDTO.Email);
            //                var existingAccountsByPhone = await _accountRepository.GetAccountsByPhoneNumberList(sponsorDTO.PhoneNumber);

            //                int accountId = 0;
            //                var matchedAccount = existingAccountsByEmail.FirstOrDefault(e => existingAccountsByPhone.Any(p => p.Id == e.Id));

            //                // Check if account exists by both email and phone
            //                if (matchedAccount != null)
            //                {
            //                    accountId = matchedAccount.Id;
            //                }
            //                else
            //                {
            //                    var accountDTO = new AccountDTO
            //                    {
            //                        Name = sponsorDTO.Name,
            //                        Email = sponsorDTO.Email,
            //                        Username = sponsorDTO.Email,
            //                        Password = "1", // Default password
            //                        StudentId = "", // Or any default/required value
            //                        PhoneNumber = sponsorDTO.PhoneNumber,
            //                        Dob = DateTime.Now,
            //                        Gender = "Others", // Or set appropriately
            //                        AvatarUrl = null,
            //                        AccountStatus = "Active",
            //                        RoleId = 3, // Sponsor role
            //                        SubjectId = null
            //                    };

            //                    _emailHelper.SendEmailToNewAccount(accountDTO.Email, accountDTO.Username, accountDTO.Password);

            //                    var accountCreate = _mapper.Map<Account>(accountDTO);
            //                    var createdAccount = await _accountRepository.CreateAccount(accountCreate);

            //                    if (createdAccount != null)
            //                    {
            //                        accountId = createdAccount.Id;
            //                    }
            //                    else
            //                    {
            //                        return new BaseResponse
            //                        {
            //                            StatusCode = 500,
            //                            Message = "Account creation failed",
            //                            IsSuccess = false
            //                        };
            //                    }
            //                }

            //                var newSponsor = _mapper.Map<Sponsor>(sponsorDTO);
            //                newSponsor.AccountId = accountId;
            //                newSponsor.AvatarUrl = null; // Manually set AvatarUrl to null
            //                var sponsorResult = await _sponsorRepository.AddSponsorAsync(newSponsor);

            //                if (!sponsorResult)
            //                {
            //                    return new BaseResponse
            //                    {
            //                        StatusCode = 500,
            //                        Message = "Sponsor creation failed",
            //                        IsSuccess = false
            //                    };
            //                }
            //            }
            //        }
            //    }
            //}

            // Map the DTO to the Event entity

            Event eventCreate = new Event
            {
                Name = createEventDTO.Name,
                StartSellDate = createEventDTO.StartSellDate,
                EndSellDate = createEventDTO.EndSellDate,
                Price = createEventDTO.Price,
                Quantity = createEventDTO.Quantity,
                Remaining = createEventDTO.Quantity,
                AvatarUrl = null,
                EventStatus = EventStatus.Pending,
                Description = createEventDTO.Description,
                OwnerId = createEventDTO.OwnerId,
                SubjectId = createEventDTO.SubjectId
            };

            //var newEvent = _mapper.Map<Event>(createEventDTO);
            //newEvent.Remaining = createEventDTO.Quantity;

            // Set the EventStatus to Pending
            //newEvent.EventStatus = EventStatus.Pending;

            var eventResult = await _eventRepository.Add(eventCreate);

            if (!eventResult)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Message = "Event create failed",
                    IsSuccess = false
                };
            }
            else
            {
                foreach (var scheduleDTO in createEventDTO.ScheduleList)
                {
                    var newEventSchedule = _mapper.Map<EventSchedule>(scheduleDTO);
                    newEventSchedule.EventId = eventCreate.Id;
                    await _eventScheduleRepository.AddEventScheduleAsync(newEventSchedule);
                }

                // Add the event to the repository and save changes to generate EventId

                //      await _sponsorRepository.DeleteDuplicateSponsorsAsync();
                //       await _sponsorRepository.DeleteSponsorsWithNullAccountIdAsync();

                if (createEventDTO.Sponsorships != null)
                {
                    foreach (var sponsorshipDTO in createEventDTO.Sponsorships)
                    {
                        var sponsorDTO = sponsorshipDTO.Sponsor;
                        if (sponsorDTO != null)
                        {
                            // Validate email format
                            if (!ValidationUtils.IsValidEmail(sponsorDTO.Email))
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 200,
                                    Message = "Create event successfully but sponsor failed because of invalid email format",
                                    Data = _mapper.Map<Event>(eventCreate),
                                    IsSuccess = true
                                };
                            }

                            // Validate phone number format
                            if (!ValidationUtils.IsPhoneNumber(sponsorDTO.PhoneNumber))
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 200,
                                    Message = "Create event successfully but sponsor failed because of invalid phone number format",
                                    Data = _mapper.Map<Event>(eventCreate),
                                    IsSuccess = true
                                };
                            }

                            var existEmailSponsors = await _sponsorRepository.GetSponsorByEmailAsync(sponsorDTO.Email);
                            var existPhoneSponsors = await _sponsorRepository.GetSponsorByPhoneNumberAsync(sponsorDTO.PhoneNumber);

                            if (existPhoneSponsors == null && existEmailSponsors == null)
                            {
                                var existingAccountsByEmail = await _accountRepository.GetAccountsByEmailList(sponsorDTO.Email);
                                var existingAccountsByPhone = await _accountRepository.GetAccountsByPhoneNumberList(sponsorDTO.PhoneNumber);

                                int accountId = 0;
                                var matchedAccount = existingAccountsByEmail.FirstOrDefault(e => existingAccountsByPhone.Any(p => p.Id == e.Id));

                                // Check if account exists by both email and phone
                                if (matchedAccount != null)
                                {
                                    accountId = matchedAccount.Id;
                                }
                                else
                                {
                                    var accountDTO = new AccountDTO
                                    {
                                        Name = sponsorDTO.Name,
                                        Email = sponsorDTO.Email,
                                        Username = sponsorDTO.Email,
                                        Password = "123456", // Default password
                                        StudentId = "", // Or any default/required value
                                        PhoneNumber = sponsorDTO.PhoneNumber,
                                        Dob = DateTime.Now,
                                        Gender = "Others", // Or set appropriately
                                        AvatarUrl = null,
                                        AccountStatus = "Active",
                                        RoleId = 3, // Sponsor role
                                        SubjectId = null
                                    };

                                    var accountCreate = _mapper.Map<Account>(accountDTO);
                                    var createdAccount = await _accountRepository.CreateAccount(accountCreate);

                                    if (createdAccount != null)
                                    {
                                        accountId = createdAccount.Id;
                                        _emailHelper.SendEmailToNewAccount(accountDTO.Email, accountDTO.Username, accountDTO.Password);
                                    }
                                    else
                                    {
                                        return new BaseResponse
                                        {
                                            StatusCode = 200,
                                            Message = "Create event successfully but sponsor failed",
                                            Data = _mapper.Map<Event>(eventCreate),
                                            IsSuccess = true
                                        };
                                    }
                                }

                                var newSponsor = _mapper.Map<Sponsor>(sponsorDTO);
                                newSponsor.AccountId = accountId;
                                newSponsor.AvatarUrl = null; // Manually set AvatarUrl to null
                                var sponsorResult = await _sponsorRepository.AddSponsorAsync(newSponsor);

                                if (!sponsorResult)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 200,
                                        Message = "Create event successfully but sponsor failed",
                                        Data = _mapper.Map<Event>(eventCreate),
                                        IsSuccess = true
                                    };
                                }
                                else
                                {
                                    var sum = sponsorshipDTO.Sum;
                                    string type;
                                    if (sum < 1000000)
                                    {
                                        type = "Bronze";
                                    }
                                    else if (1000000 <= sum && sum <= 5000000)
                                    {
                                        type = "Silver";
                                    }
                                    else if (5000000 <= sum && sum <= 10000000)
                                    {
                                        type = "Gold";
                                    }
                                    else
                                    {
                                        type = "Platinum";
                                    }

                                    var sponsorship = new Sponsorship
                                    {
                                        EventId = eventCreate.Id,
                                        SponsorId = newSponsor.Id,
                                        Description = string.Empty,
                                        Type = type,
                                        Title = sponsorshipDTO.Title,
                                        Sum = sponsorshipDTO.Sum
                                    };

                                    // Add the sponsorship to the repository
                                    var sponsorshipResult = await _sponsorshipRepository.CreateSponsorship(sponsorship);

                                    if (!sponsorshipResult)
                                    {
                                        return new BaseResponse
                                        {
                                            StatusCode = 200,
                                            Message = "Create event successfully but sponsorship failed",
                                            Data = _mapper.Map<Event>(eventCreate),
                                            IsSuccess = true
                                        };
                                    }
                                }
                            }
                            else
                            {
                                var sum = sponsorshipDTO.Sum;
                                string type;
                                if (sum < 1000000)
                                {
                                    type = "Bronze";
                                }
                                else if (1000000 <= sum && sum <= 5000000)
                                {
                                    type = "Silver";
                                }
                                else if (5000000 <= sum && sum <= 10000000)
                                {
                                    type = "Gold";
                                }
                                else
                                {
                                    type = "Platinum";
                                }
                                var sponsorship = new Sponsorship
                                {
                                    EventId = eventCreate.Id,
                                    SponsorId = existEmailSponsors.Id,
                                    Description = string.Empty,
                                    Type = type,
                                    Title = sponsorshipDTO.Title,
                                    Sum = sponsorshipDTO.Sum
                                };

                                // Add the sponsorship to the repository
                                var sponsorshipResult = await _sponsorshipRepository.CreateSponsorship(sponsorship);

                                if (!sponsorshipResult)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 200,
                                        Message = "Create event successfully but sponsorship failed",
                                        Data = _mapper.Map<Event>(eventCreate),
                                        IsSuccess = true
                                    };
                                }
                            }

                        }
                    }
                }

                var eventDTO = _mapper.Map<EventDTO>(eventCreate);

                return new BaseResponse
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Message = "Create succesfully",
                    Data = eventDTO
                };
            }
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
                List<SponsorshipsWithSponsorsDTO> sponsorshipWithSponsors = new List<SponsorshipsWithSponsorsDTO>();
                foreach(var e in sponsorships)
                {
                    var sponsors = await _sponsorRepository.GetSponsorByIdAsync(e.SponsorId);
                    SponsorshipsWithSponsorsDTO sponsorshipEntity = new SponsorshipsWithSponsorsDTO
                    {
                        Id = e.Id,
                        Description = e.Description,
                        Type = e.Type,
                        Title = e.Title,
                        Sum = e.Sum,
                        SponsorId = e.SponsorId,
                        EventId = e.EventId,
                        Sponsor = _mapper.Map<SponsorDTO>(sponsors)
                    };

                    sponsorshipWithSponsors.Add(sponsorshipEntity);
                }

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
                    Sponsorships = sponsorshipWithSponsors
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

        public async Task<BaseResponse> UpdateEventDetails(int id, UpdateEventDTO updateEventDTO)
        {
            // Validate that no properties contain only whitespace
            try
            {
                ValidationUtils.ValidateNoWhitespaceOnly(updateEventDTO);
            }
            catch (ArgumentException ex)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }

            // Check if ScheduleList is not empty
            if (updateEventDTO.ScheduleList == null || !updateEventDTO.ScheduleList.Any())
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "ScheduleList cannot be empty",
                    IsSuccess = false
                };
            }

            // Validation: StartSellDate < EndSellDate
            if (updateEventDTO.StartSellDate >= updateEventDTO.EndSellDate)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "StartSellDate must be before EndSellDate",
                    IsSuccess = false
                };
            }

            // Check for duplicate schedules within the list
            var scheduleTimes = new HashSet<(string Place, DateTime StartTime, DateTime EndTime)>();
            foreach (var schedule in updateEventDTO.ScheduleList)
            {
                if (!scheduleTimes.Add((schedule.Place, schedule.StartTime, schedule.EndTime)))
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Message = "Duplicate schedules are not allowed",
                        IsSuccess = false
                    };
                }
            }

            // Check for overlapping schedules within the user's input list
            for (int i = 0; i < updateEventDTO.ScheduleList.Count; i++)
            {
                for (int j = i + 1; j < updateEventDTO.ScheduleList.Count; j++)
                {
                    var schedule1 = updateEventDTO.ScheduleList[i];
                    var schedule2 = updateEventDTO.ScheduleList[j];
                    if (schedule1.Place == schedule2.Place &&
                        schedule1.StartTime < schedule2.EndTime &&
                        schedule1.EndTime > schedule2.StartTime)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 400,
                            Message = "Schedules within the list overlap",
                            IsSuccess = false
                        };
                    }
                }
            }

            // Check for overlapping events at the same place and time in the database
            foreach (var scheduleDTO in updateEventDTO.ScheduleList)
            {
                // Validation: EndSellDate < StartTime
                if (updateEventDTO.EndSellDate >= scheduleDTO.StartTime)
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

            var eventEntity = await _eventRepository.GetEventById(id);
            if (eventEntity == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Event not found",
                    IsSuccess = false
                };
            }

            // Map updates to the event entity
            eventEntity.Name = updateEventDTO.Name;
            eventEntity.StartSellDate = updateEventDTO.StartSellDate;
            eventEntity.EndSellDate = updateEventDTO.EndSellDate;
            eventEntity.Price = updateEventDTO.Price;
            eventEntity.Quantity = updateEventDTO.Quantity;
            eventEntity.Remaining = updateEventDTO.Remaining;
            eventEntity.Description = updateEventDTO.Description;
            eventEntity.OwnerId = updateEventDTO.OwnerId;
            eventEntity.SubjectId = updateEventDTO.SubjectId;
            eventEntity.AvatarUrl = updateEventDTO.AvatarUrl;
            eventEntity.EventStatus = Enum.Parse<EventStatus>(updateEventDTO.EventStatus);

            if (updateEventDTO.ScheduleList != null && updateEventDTO.ScheduleList.Any())
            {
                // Update event schedules
                await _eventScheduleRepository.DeleteSchedulesByEventId(eventEntity.Id);
                foreach (var scheduleDTO in updateEventDTO.ScheduleList)
                {
                    var newEventSchedule = _mapper.Map<EventSchedule>(scheduleDTO);
                    newEventSchedule.EventId = eventEntity.Id;
                    await _eventScheduleRepository.AddEventScheduleAsync(newEventSchedule);
                }
            }

            await _eventRepository.UpdateEvent(eventEntity);

            return new BaseResponse
            {
                StatusCode = 200,
                Message = "Event updated successfully",
                IsSuccess = true
            };
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

        public async Task<BaseResponse> UploadImageForEvent(int id, IFormFile? file)
        {
            var eventEntity = await _eventRepository.GetEventById(id);
            if(eventEntity == null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    IsSuccess = true,
                    Data = null,
                    Message = "Event unfound"
                };
            }
            else
            {
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(eventEntity.AvatarUrl))
                    {
                        await _cloudinaryHelper.DeleteImageAsync(eventEntity.AvatarUrl);
                    }

                    var imageUrl = await _cloudinaryHelper.UploadImageAsync(file);
                    eventEntity.AvatarUrl = imageUrl;
                }
                else
                {
                    eventEntity.AvatarUrl = eventEntity.AvatarUrl;
                }

                await _eventRepository.UpdateEvent(eventEntity);
                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = eventEntity,
                    Message = "Upload successfully"
                };
            }
        }
    }
}
