using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<List<Sponsorship>> GetAllSponsorships(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Sponsorship> query = _context.Sponsorships;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortOrder))
            {
                Expression<Func<Sponsorship, object>> keySelector = sortColumn switch
                {
                    "sum" => e => e.Sum,
                    "sponsor" => e => e.Sponsor,
                    "title" => e => e.Title,
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

            var sponsorships = await query.ToListAsync();
            return sponsorships;
        }
    

        public async Task<Sponsorship> GetSponsorshipById(int id)
        {
            return await _context.Sponsorships.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> UpdateSponsorship(Sponsorship sponsorship)
        {
            _context.Sponsorships.Update(sponsorship);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Sponsorship>> GetAllSponsorshipsByEventId(int id)
        {
            return await _context.Sponsorships.Where(e => e.EventId == id).ToListAsync();
        }

        public async Task<bool> DeleteSponsorship(int id)
        {
            var sponsorship = await _context.Sponsorships.FirstOrDefaultAsync(e => e.Id == id);
            if(sponsorship != null)
            {
                _context.Sponsorships.Remove(sponsorship);
                return await _context.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }

        }

        public async Task<List<Sponsorship>> GetSponsorshipBySponsorId(int id, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            IQueryable<Sponsorship> query = _context.Sponsorships.Where(e => e.SponsorId == id);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortOrder))
            {
                Expression<Func<Sponsorship, object>> keySelector = sortColumn switch
                {
                    "sum" => e => e.Sum,
                    "sponsor" => e => e.Sponsor,
                    "title" => e => e.Title,
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

            var sponsorships = await query.ToListAsync();
            return sponsorships;
        }
    }
}
