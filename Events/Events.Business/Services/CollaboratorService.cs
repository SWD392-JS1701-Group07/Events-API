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

namespace Events.Business.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _collaboratorRepository;
        private readonly IMapper _mapper;

        public CollaboratorService(ICollaboratorRepository collaboratorRepository, IMapper mapper)
        {
            _collaboratorRepository = collaboratorRepository;
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
