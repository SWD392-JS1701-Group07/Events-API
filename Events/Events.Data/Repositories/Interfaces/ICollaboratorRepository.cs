using Events.Models;
using Events.Models.Models;
using Events.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ICollaboratorRepository
    {
        Task<IEnumerable<Collaborator>> GetAllCollaboratorsAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<Collaborator> GetCollaboratorById(int id);
        Task<IEnumerable<Collaborator>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus);
        Task<Collaborator> AddAsync(Collaborator collaborator);
        Task<Collaborator> GetByIdAsync(int id);
        Task UpdateAsync(Collaborator collaborator);
        Task<List<int>> GetAllEventIdByCollaboratorId(int id);
        Task<List<Collaborator>> GetAllCollaboratorsByEventId(int id, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<List<Collaborator>> GetAllCollaboratorsByEventIdWithoutFilter(int id);
        Task<Collaborator> GetCollaboratorByEventAndAccount(int eventId, int accountId);
        Task<List<Event>> GetEventsByCollaboratorAccount(int accountId);
        Task<bool> UpdateCollaborators(Collaborator collaborator);

    }
}