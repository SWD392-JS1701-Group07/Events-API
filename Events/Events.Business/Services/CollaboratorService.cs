using AutoMapper;
using Events.Business.Interfaces;
using Events.Data;
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
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _collaboratorRepository;
        private readonly IMapper _mapper;

        public CollaboratorService(ICollaboratorRepository collaboratorRepository, IMapper mapper)
        {
            _collaboratorRepository = collaboratorRepository;
            _mapper = mapper;
        }

        public async Task<List<CollaboratorDTO>> GetAllCollaborators()
        {
            var collaborators = await _collaboratorRepository.GetAllCollaborators();
            return _mapper.Map<List<CollaboratorDTO>>(collaborators);
        }

        public async Task<CollaboratorDTO> GetCollaboratorById(int id)
        {
            var collaborator = await _collaboratorRepository.GetCollaboratorById(id);
            return _mapper.Map<CollaboratorDTO>(collaborator);
        }

        public async Task<List<CollaboratorDTO>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var collaborators = await _collaboratorRepository.SearchCollaborators(accountId, eventId, collabStatus);
            return _mapper.Map<List<CollaboratorDTO>>(collaborators);
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
                CollabStatus = createdCollaborator.CollabStatus,
                IsCheckIn = createdCollaborator.IsCheckIn
            };
        }
    }
}
