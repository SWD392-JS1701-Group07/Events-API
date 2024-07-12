using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ISponsorRepository
    {
        Task<IEnumerable<Sponsor>> GetAllSponsor(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<Sponsor> GetSponsorByIdAsync(int id);
        Task<bool> UpdateSponsorAsync(Sponsor sponsor);
        Task<bool> AddSponsorAsync(Sponsor sponsor);
        Task<bool> DeleteSponsorAsync(Sponsor sponsor);
        Task<Sponsor> GetSponsorByEmailAsync(string email);
        Task<Sponsor> GetSponsorByPhoneNumberAsync(string phoneNumber);
        Task<Sponsor> GetSponsorByAccountId(int id);
        Task<int> SaveChangesAsync();
        Task DeleteSponsorsWithNullAccountIdAsync();
        Task DeleteDuplicateSponsorsAsync();
    }
}
