using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ISponsorshipRepository
    {
        Task<List<Sponsorship>> GetAllSponsorships();
        Task<Sponsorship> GetSponsorshipById(int id);
        Task<List<Sponsorship>> GetAllSponsorshipsByEventId(int id);
        Task<bool> CreateSponsorship(Sponsorship sponsorship);
        Task<bool> UpdateSponsorship(Sponsorship sponsorship);
    }
}
