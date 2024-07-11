using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<BaseResponse> DeleteSponsorship(int id)
        {
            var sponsorships = await _sponsorshipRepository.GetSponsorshipById(id);
            if(sponsorships == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Sponsorship unfound"
                };
            }
            else
            {
                var result = await _sponsorshipRepository.DeleteSponsorship(id);
                if (result)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        Data = null,
                        IsSuccess = true,
                        Message = "Delete successfully"
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

        public async Task<BaseResponse> UpdateSponsorship(int id, CreateSponsorshipDTO createSponsorshipDTO)
        {
            if (createSponsorshipDTO.Title.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "The title can't be empty"
                };
            } else if (createSponsorshipDTO.Type.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "The type can't be empty"
                };
            }
            else
            {
                var sponsorshipExist = await _sponsorshipRepository.GetSponsorshipById(id);
                if (sponsorshipExist == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 404,
                        Data = null,
                        IsSuccess = false,
                        Message = "Unfound"
                    };
                }
                else
                {
                    sponsorshipExist.Title = createSponsorshipDTO.Title;
                    sponsorshipExist.Sum = createSponsorshipDTO.Sum;
                    sponsorshipExist.Description = createSponsorshipDTO.Description;
                    sponsorshipExist.Type = createSponsorshipDTO.Type;

                    var result = await _sponsorshipRepository.UpdateSponsorship(sponsorshipExist);

                    if (result)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 200,
                            Data = null,
                            IsSuccess = true,
                            Message = "Updated successfully"
                        };
                    }
                    else
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = true,
                            Message = "Something went wrong"
                        };
                    }
                }
            }  
        }
    }
}
