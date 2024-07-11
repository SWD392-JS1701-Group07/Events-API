using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
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

		public async Task<Sponsor> GetSponsorByIdAsync(int id) => await _context.Sponsors.FindAsync(id) ?? throw new KeyNotFoundException("Sponsor not found!");

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
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<Sponsor> GetSponsorByEmailAsync(string email)
        {
            return await _context.Sponsors.FirstOrDefaultAsync(s => s.Email == email);
        }
        public async Task DeleteSponsorsWithNullAccountIdAsync()
        {
            var sponsorsToDelete = await _context.Sponsors
                .Where(s => s.AccountId == null)
                .ToListAsync();

            if (sponsorsToDelete.Any())
            {
                var sponsorIds = sponsorsToDelete.Select(s => s.Id).ToList();
                var sponsorshipsToDelete = await _context.Sponsorships
                    .Where(sp => sponsorIds.Contains(sp.SponsorId))
                    .ToListAsync();

                _context.Sponsorships.RemoveRange(sponsorshipsToDelete);
                _context.Sponsors.RemoveRange(sponsorsToDelete);

                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteDuplicateSponsorsAsync()
        {
            var duplicateEmails = await _context.Sponsors
                .GroupBy(s => s.Email)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToListAsync();

            foreach (var email in duplicateEmails)
            {
                var duplicateSponsors = await _context.Sponsors
                    .Where(s => s.Email == email)
                    .OrderBy(s => s.Id)
                    .ToListAsync();

                var sponsorToKeep = duplicateSponsors.First();
                var sponsorsToDelete = duplicateSponsors.Skip(1).ToList();

                if (sponsorsToDelete.Any())
                {
                    var sponsorIdsToDelete = sponsorsToDelete.Select(s => s.Id).ToList();
                    var sponsorshipsToDelete = await _context.Sponsorships
                        .Where(sp => sponsorIdsToDelete.Contains(sp.SponsorId))
                        .ToListAsync();

                    _context.Sponsorships.RemoveRange(sponsorshipsToDelete);
                    _context.Sponsors.RemoveRange(sponsorsToDelete);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Sponsor> GetSponsorByAccountId(int id)
        {
            return await _context.Sponsors.FirstOrDefaultAsync(e => e.AccountId == id);
        }

        public async Task<Sponsor> GetSponsorByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Sponsors.FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber);
        }
    }
}
