using Events.Data.DTOs.Request;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ISponsorService
    {
        Task<BaseResponse> GetAllSponsor();
        Task<BaseResponse> GetSponsorByIdAsync(int id);
        Task<BaseResponse> AddSponsorAsync(CreateSponsorDTO sponsorDto);
        Task<BaseResponse> UpdateSponsorAsync(int id, UpdateSponsorDTO sponsorDto);
        Task<BaseResponse> DeleteSponsorAsync(int id);
    }
}
