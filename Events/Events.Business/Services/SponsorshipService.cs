using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
    public class SponsorshipService : ISponsorshipService
    {
        private readonly ISponsorshipRepository _sponsorshipRepository;
        private readonly IMapper _mapper;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly IEventRepository _eventRepository;

        public SponsorshipService(ISponsorshipRepository sponsorshipRepository, IMapper mapper, ISponsorRepository sponsorRepository, IEventRepository eventRepository)
        {
            _sponsorshipRepository = sponsorshipRepository;
            _mapper = mapper;
            _sponsorRepository = sponsorRepository;
            _eventRepository = eventRepository;
        }

        public async Task<BaseResponse> CreateSponsorship(CreateSponsorshipDTO createSponsorshipDTO)
        {
            var sponsor = await _sponsorRepository.GetSponsorByIdAsync(createSponsorshipDTO.SponsorId);
            var eventExist = await _eventRepository.GetEventById(createSponsorshipDTO.EventId);
            if(sponsor == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can't found this sponsor"
                };
            } else if(eventExist == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can't found this event"
                };
            }
            else
            {
                SponsorshipDTO sponsorship = new SponsorshipDTO
                {
                    Description = createSponsorshipDTO.Description,
                    Type = createSponsorshipDTO.Type,
                    Title = createSponsorshipDTO.Title,
                    Sum = createSponsorshipDTO.Sum,
                    SponsorId = createSponsorshipDTO.SponsorId,
                    EventId = createSponsorshipDTO.EventId,
                };

                var result = await _sponsorshipRepository.CreateSponsorship(_mapper.Map<Sponsorship>(sponsorship));
                if (result)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        Data = sponsorship,
                        IsSuccess = true,
                        Message = "Craeted successfully"
                    };
                }
                else
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "Something went wrong"
                    };
                }
            }
        }

        public async Task<BaseResponse> GetAllSponsorship()
        {
            var sponsorships = await _sponsorshipRepository.GetAllSponsorships();
            var results = _mapper.Map<List<SponsorshipDTO>>(sponsorships);
            return results.Any() ? new BaseResponse
            {
                StatusCode = 200,
                Data = results,
                IsSuccess = true,
                Message = "Return successfully"
            } :
            new BaseResponse
            {
                StatusCode = 404,
                Data = null,
                IsSuccess = false,
                Message = "Unfound"
            };
        }

        public async Task<BaseResponse> GetSponsorshipById(int id)
        {
            var sponsorship = await _sponsorshipRepository.GetSponsorshipById(id);
            var results = _mapper.Map<SponsorshipDTO>(sponsorship);
            if(results == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Unfound"
                };
            } else
            {
                return new BaseResponse
                {
                    StatusCode = 200,
                    Data = results,
                    IsSuccess = true,
                    Message = "Return successfully"
                };
            }  
        }
    }
}
