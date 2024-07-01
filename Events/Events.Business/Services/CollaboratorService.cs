using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models;
using Events.Utils;
using Events.Data.Repositories;

namespace Events.Business.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _collaboratorRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventScheduleRepository _eventScheduleRepository;
        private readonly IMapper _mapper;

        public CollaboratorService(ICollaboratorRepository collaboratorRepository,
                                   IEventRepository eventRepository,
                                   IEventScheduleRepository eventScheduleRepository,
                                   IMapper mapper)
        {
            _collaboratorRepository = collaboratorRepository;
            _eventRepository = eventRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CollaboratorDTO>> GetAllCollaborators()
        {
            var collaborators = await _collaboratorRepository.GetAllCollaboratorsAsync();
            return collaborators.Select(c => new CollaboratorDTO
            {
                Id = c.Id,
                IsCheckIn = c.IsCheckIn,
                AccountId = c.AccountId,
                EventId = c.EventId,
                EventName = c.Event.Name,
                CollabStatus = c.CollabStatus.ToString() 
            }).ToList();
        }

        public async Task<CollaboratorDTO> GetCollaboratorById(int id)
        {
            var collaborator = await _collaboratorRepository.GetCollaboratorById(id);
            return _mapper.Map<CollaboratorDTO>(collaborator);
        }

        public async Task<IEnumerable<CollaboratorDTO>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var collaborators = await _collaboratorRepository.SearchCollaborators(accountId, eventId, collabStatus);
            return collaborators.Select(c => new CollaboratorDTO
            {
                Id = c.Id,
                IsCheckIn = c.IsCheckIn,
                AccountId = c.AccountId,
                EventId = c.EventId,
                EventName = c.Event.Name, 
                CollabStatus = c.CollabStatus.ToString()
            }).ToList();
        }

        public async Task<CollaboratorDTO> CreateCollaborator(CreateCollaboratorDTO createCollaboratorDto)
        {
            // Retrieve the event
            var eventEntity = await _eventRepository.GetEventById(createCollaboratorDto.EventId);
            if (eventEntity == null || eventEntity.EventStatus != Enums.EventStatus.Ongoing)
            {
                throw new Exception("The event is either not found or not in an ongoing status.");
            }

            // Check if the collaborator is already registered for the event
            var existingCollaborator = await _collaboratorRepository.GetCollaboratorByEventAndAccount(createCollaboratorDto.EventId, createCollaboratorDto.AccountId);
            if (existingCollaborator != null)
            {
                throw new Exception("The collaborator is already registered for this event.");
            }


            // Check for overlapping event schedules
            var newEventSchedules = await _eventScheduleRepository.GetEventScheduleById(createCollaboratorDto.EventId);
            var collaboratorEvents = await _collaboratorRepository.GetEventsByCollaboratorAccount(createCollaboratorDto.AccountId);
            foreach (var collaboratorEvent in collaboratorEvents)
            {
                var collaboratorEventSchedules = await _eventScheduleRepository.GetEventScheduleById(collaboratorEvent.Id);
                foreach (var collaboratorEventSchedule in collaboratorEventSchedules)
                {
                    foreach (var newEventSchedule in newEventSchedules)
                    {
                        if ((collaboratorEventSchedule.StartTime < newEventSchedule.EndTime && collaboratorEventSchedule.EndTime > newEventSchedule.StartTime))
                        {
                            throw new Exception("The collaborator is already registered for events with overlapping schedules.");
                        }
                    }
                }
            }

            // Create new collaborator
            var newCollaborator = new Collaborator
            {
                AccountId = createCollaboratorDto.AccountId,
                EventId = createCollaboratorDto.EventId,
                CollabStatus = Enums.CollaboratorStatus.Registered,
                IsCheckIn = 0
            };

            var createdCollaborator = await _collaboratorRepository.AddAsync(newCollaborator);

            return new CollaboratorDTO
            {
                Id = createdCollaborator.Id,
                AccountId = createdCollaborator.AccountId,
                EventId = createdCollaborator.EventId,
                EventName = eventEntity.Name,
                CollabStatus = createdCollaborator.CollabStatus.ToString(),
                IsCheckIn = createdCollaborator.IsCheckIn
            };
        } 
        public async Task<CollaboratorDTO> ApproveCollaboratorAsync(int id)
        {
            var collaborator = await _collaboratorRepository.GetByIdAsync(id);
            if (collaborator == null)
            {
                return null;
            }

            collaborator.CollabStatus = Enums.CollaboratorStatus.Approved;
            await _collaboratorRepository.UpdateAsync(collaborator);

            return new CollaboratorDTO
            {
                Id = collaborator.Id,
                IsCheckIn = collaborator.IsCheckIn,
                AccountId = collaborator.AccountId,
                EventId = collaborator.EventId,
                EventName = collaborator.Event.Name,
                CollabStatus = collaborator.CollabStatus.ToString()
            };
        }
        public async Task<CollaboratorDTO> CancelCollaboratorAsync(int id)
        {
            var collaborator = await _collaboratorRepository.GetByIdAsync(id);
            if (collaborator == null)
            {
                return null;
            }

            collaborator.CollabStatus = Enums.CollaboratorStatus.Completed;
            await _collaboratorRepository.UpdateAsync(collaborator);

            return new CollaboratorDTO
            {
                Id = collaborator.Id,
                IsCheckIn = collaborator.IsCheckIn,
                AccountId = collaborator.AccountId,
                EventId = collaborator.EventId,
                EventName = collaborator.Event.Name,
                CollabStatus = collaborator.CollabStatus.ToString()
            };
        }
        public async Task<CollaboratorDTO> RejectCollaboratorAsync(int id)
        {
            var collaborator = await _collaboratorRepository.GetByIdAsync(id);
            if (collaborator == null)
            {
                return null;
            }

            collaborator.CollabStatus = Enums.CollaboratorStatus.Rejected;
            await _collaboratorRepository.UpdateAsync(collaborator);

            return new CollaboratorDTO
            {
                Id = collaborator.Id,
                IsCheckIn = collaborator.IsCheckIn,
                AccountId = collaborator.AccountId,
                EventId = collaborator.EventId,
                EventName = collaborator.Event.Name,
                CollabStatus = collaborator.CollabStatus.ToString()
            };
        }
    }
}
