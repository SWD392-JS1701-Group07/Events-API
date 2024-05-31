using Events.Data;
using Events.Data.DTOs;
using Events.Data.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Interfaces
{
    public interface ICollaboratorService
    {
        Task<List<CollaboratorDTO>> GetAllCollaborators();
        Task<CollaboratorDTO> GetCollaboratorById(int id);
        Task<List<CollaboratorDTO>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus);
        Task<CollaboratorDTO> CreateCollaborator(CreateCollaboratorDTO createCollaboratorDto);
    }
}