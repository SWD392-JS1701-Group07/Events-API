using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Interfaces
{
	public interface ISponsorRepository
	{
		Task<IEnumerable<Sponsor>> GetAllSponsor();
		Task<Sponsor> GetSponsorByIdAsync(int id);
		Task<bool> UpdateSponsorAsync(Sponsor sponsor);
		Task<bool> AddSponsorAsync(Sponsor sponsor);
		Task<bool> DeleteSponsorAsync(Sponsor sponsor);
	}
}
