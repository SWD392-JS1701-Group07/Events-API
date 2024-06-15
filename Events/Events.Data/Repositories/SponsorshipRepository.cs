using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class SponsorshipRepository : ISponsorshipRepository
    {
        private readonly EventsDbContext _context;

        public SponsorshipRepository(EventsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateSponsorship(Sponsorship sponsorship)
        {
            _context.Sponsorships.AddAsync(sponsorship);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Sponsorship>> GetAllSponsorships()
        {
            return await _context.Sponsorships.ToListAsync();
        }

        public async Task<Sponsorship> GetSponsorshipById(int id)
        {
            return await _context.Sponsorships.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
