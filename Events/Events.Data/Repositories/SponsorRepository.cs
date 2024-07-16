using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Events.Data.Repositories
{
    public class SponsorRepository : ISponsorRepository
	{
		private readonly EventsDbContext _context;
        public SponsorRepository(EventsDbContext context)
        {
            _context = context;
        }

		public async Task<IEnumerable<Sponsor>> GetAllSponsor(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Sponsor> query = _context.Sponsors;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortOrder))
            {
                Expression<Func<Sponsor, object>> keySelector = sortColumn switch
                {
                    "name" => e => e.Name,
                    "email" => e => e.Email,
                    "phoneNumber" => e => e.PhoneNumber,
                    _ => e => e.Id,
                };

                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(keySelector),
                    "desc" => query.OrderByDescending(keySelector),
                    _ => query.OrderBy(keySelector)
                };
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var sponsor = await query.ToListAsync();
            return sponsor;
        }
    

		public async Task<Sponsor> GetSponsorByIdAsync(int id) => await _context.Sponsors.FindAsync(id) ?? throw new KeyNotFoundException("Sponsor not found!");
        public async Task<Sponsor> GetSponsorById(int id) => await _context.Sponsors.FirstOrDefaultAsync(e => e.Id == id) ?? throw new KeyNotFoundException("Sponsor not found!");

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
