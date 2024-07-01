using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models;
using Events.Utils;

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
            return await _context.Collaborators
                                 .Include(c => c.Event)
                                 .FirstOrDefaultAsync(c => c.Id == id);
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

        public async Task<List<int>> GetAllEventIdByCollaboratorId(int id)
        {
            var eventList = await _context.Collaborators.Where(e => e.AccountId == id).Select(e => e.Event.Id).ToListAsync();
            return eventList;
        }

        public async Task<List<Collaborator>> GetAllCollaboratorsByEventId(int id)
        {
            return await _context.Collaborators.Where(e => e.EventId == id).ToListAsync();
        }

        public async Task<Collaborator> GetCollaboratorByEventAndAccount(int eventId, int accountId)
        {
            return await _context.Collaborators
                                 .FirstOrDefaultAsync(c => c.EventId == eventId && c.AccountId == accountId);
        }

        public async Task<List<Event>> GetEventsByCollaboratorAccount(int accountId)
        {
            return await (from e in _context.Events
                          join c in _context.Collaborators on e.Id equals c.EventId
                          where c.AccountId == accountId
                          select e).ToListAsync();
        }

        public async Task<bool> UpdateCollaborators(Collaborator collaborator)
        {
           _context.Collaborators.Update(collaborator);
           return await _context.SaveChangesAsync() > 0;
        }
    }
}