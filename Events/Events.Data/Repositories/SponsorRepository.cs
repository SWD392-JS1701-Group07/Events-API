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
	public class SponsorRepository : ISponsorRepository
	{
		private readonly EventsDbContext _context;
        public SponsorRepository(EventsDbContext context)
        {
            _context = context;
        }

		public async Task<IEnumerable<Sponsor>> GetAllSponsor() => await _context.Sponsors.ToListAsync();

		public async Task<Sponsor> GetSponsorByIdAsync(int id) => await _context.Sponsors.FindAsync(id);

		public async Task<bool> AddSponsorAsync(Sponsor sponsor)
		{
			_context.Sponsors.Add(sponsor);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteSponsorAsync(Sponsor sponsor)
		{
			_context.Sponsors.Remove(sponsor);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateSponsorAsync(Sponsor sponsor)
		{
			_context.Entry(sponsor).State = EntityState.Modified;
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
