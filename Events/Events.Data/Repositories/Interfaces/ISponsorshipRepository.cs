using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ISponsorshipRepository
    {
        Task<List<Sponsorship>> GetAllSponsorships(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<Sponsorship> GetSponsorshipById(int id);
        Task<List<Sponsorship>> GetAllSponsorshipsByEventId(int id);
        Task<bool> CreateSponsorship(Sponsorship sponsorship);
        Task<bool> UpdateSponsorship(Sponsorship sponsorship);
        Task<bool> DeleteSponsorship(int id);
    }
}
