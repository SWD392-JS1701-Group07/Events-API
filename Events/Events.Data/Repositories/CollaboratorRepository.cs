using Events.Data.Interfaces;
using Events.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly EventsDbContext _context;

        public CollaboratorRepository(EventsDbContext eventsDbContext)
        {
            _context = eventsDbContext;
        }

        public async Task<List<Collaborator>> GetAllCollaborators()
        {
            return await _context.Set<Collaborator>().ToListAsync();
        }

        public async Task<Collaborator> GetCollaboratorById(int id)
        {
            return await _context.Set<Collaborator>().FindAsync(id);
        }

        public async Task<IEnumerable<Collaborator>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var query = _context.Collaborators.AsQueryable();

            if (accountId.HasValue)
            {
                query = query.Where(c => c.AccountId == accountId.Value);
            }

            if (eventId.HasValue)
            {
                query = query.Where(c => c.EventId == eventId.Value);
            }

            if (collabStatus.HasValue)
            {
                query = query.Where(c => c.CollabStatus == collabStatus.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<Collaborator> AddAsync(Collaborator collaborator)
        {
            await _context.Set<Collaborator>().AddAsync(collaborator);
            await _context.SaveChangesAsync();
            return collaborator;
        }
    }
}