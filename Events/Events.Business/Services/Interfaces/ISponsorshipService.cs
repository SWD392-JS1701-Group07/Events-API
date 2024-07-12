using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ISponsorshipService
    {
        Task<BaseResponse> GetAllSponsorship(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize);
        Task<BaseResponse> GetSponsorshipById(int id);
        Task<BaseResponse> CreateSponsorship(CreateSponsorshipDTO createSponsorshipDTO);
        Task<BaseResponse> UpdateSponsorship(int id, CreateSponsorshipDTO createSponsorshipDTO);
        Task<BaseResponse> DeleteSponsorship(int id);
    }
}
