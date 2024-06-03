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
        Task<IEnumerable<CollaboratorDTO>> GetAllCollaborators();
        Task<CollaboratorDTO> GetCollaboratorById(int id);
        Task<IEnumerable<CollaboratorDTO>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus);
        Task<CollaboratorDTO> CreateCollaborator(CreateCollaboratorDTO createCollaboratorDto);
        Task<CollaboratorDTO> ApproveCollaboratorAsync(int id);
        Task<CollaboratorDTO> CancelCollaboratorAsync(int id);
        Task<CollaboratorDTO> RejectCollaboratorAsync(int id);
    }
}