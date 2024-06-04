using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models;

namespace Events.Data.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly EventsDbContext _context;

        public CollaboratorRepository(EventsDbContext eventsDbContext)
        {
            _context = eventsDbContext;
        }

        public async Task<IEnumerable<Collaborator>> GetAllCollaboratorsAsync()
        {
            return await _context.Collaborators.Include(c => c.Event).ToListAsync();
        }


        public async Task<Collaborator> GetCollaboratorById(int id)
        {
            return await _context.Set<Collaborator>().FindAsync(id);
        }

        public async Task<IEnumerable<Collaborator>> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var query = _context.Collaborators.Include(c => c.Event).AsQueryable();

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
				query = query.Where(c => c.CollabStatus == collabStatus);
            }

            return await query.ToListAsync();
        }
        public async Task<Collaborator> AddAsync(Collaborator collaborator)
        {
            await _context.Collaborators.AddAsync(collaborator);
            await _context.SaveChangesAsync();
            return collaborator;
        }
        public async Task<Collaborator> GetByIdAsync(int id)
        {
            return await _context.Collaborators.Include(c => c.Event).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Collaborator collaborator)
        {
            _context.Collaborators.Update(collaborator);
            await _context.SaveChangesAsync();
        }
    }
}