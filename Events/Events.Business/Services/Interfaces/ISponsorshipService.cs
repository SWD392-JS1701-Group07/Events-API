using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ISponsorshipService
    {
        Task<BaseResponse> GetAllSponsorship();
        Task<BaseResponse> GetSponsorshipById(int id);
        Task<BaseResponse> CreateSponsorship(CreateSponsorshipDTO createSponsorshipDTO);
        Task<BaseResponse> UpdateSponsorship(int id, CreateSponsorshipDTO createSponsorshipDTO);
        Task<BaseResponse> DeleteSponsorship(int id);
    }
}
