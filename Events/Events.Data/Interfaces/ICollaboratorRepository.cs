using Events.Data.DTOs;
using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Interfaces
{
    public interface ICollaboratorRepository
    {
        Task<List<Collaborator>> GetAllCollaborators();
        Task<Collaborator> GetCollaboratorById(int id);
        Task<IEnumerable<Collaborator>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus);
        Task<Collaborator> AddAsync(Collaborator collaborator);
    }
}