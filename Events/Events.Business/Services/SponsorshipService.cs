using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
            if(createSponsorshipDTO.Sum <= 0)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The sum value is not valid"
                };
            } else if (createSponsorshipDTO.Title.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The title value is not valid"
                };
            }
            else
            {
                var sponsor = await _sponsorRepository.GetSponsorByIdAsync(createSponsorshipDTO.SponsorId);
                var eventExist = await _eventRepository.GetEventById(createSponsorshipDTO.EventId);
                if (sponsor == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 404,
                        Data = null,
                        IsSuccess = false,
                        Message = "Can't found this sponsor"
                    };
                }
                else if (eventExist == null)
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
                    var sum = createSponsorshipDTO.Sum;
                    string type;
                    if (sum < 1000000)
                    {
                        type = "Bronze";
                    }
                    else if (1000000 <= sum && sum <= 5000000)
                    {
                        type = "Silver";
                    }
                    else if (5000000 <= sum && sum <= 10000000)
                    {
                        type = "Gold";
                    }
                    else
                    {
                        type = "Platinum";
                    }

                    SponsorshipDTO sponsorship = new SponsorshipDTO
                    {
                        Description = createSponsorshipDTO.Description,
                        Type = type,
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

        public async Task<BaseResponse> GetAllSponsorship(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var sponsorships = await _sponsorshipRepository.GetAllSponsorships(searchTerm, sortColumn, sortOrder, page, pageSize);
            List<SponsorshipsWithEventName> results = new List<SponsorshipsWithEventName>();
            foreach (var e in sponsorships)
            {
                var eventNames = await _eventRepository.GetEventById(e.EventId);
                SponsorshipsWithEventName sponsorshipsWithEvent = new SponsorshipsWithEventName
                {
                    Id = e.Id,
                    Description = e.Description,
                    Type = e.Type,
                    Title = e.Title,
                    Sum = e.Sum,
                    SponsorId = e.SponsorId,
                    EventId = e.EventId,
                    EventName = eventNames.Name
                };

                results.Add(sponsorshipsWithEvent);
            }
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

        public async Task<BaseResponse> GetSponsorshipBySponsorId(int id, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var sponsorship = await _sponsorshipRepository.GetSponsorshipBySponsorId(id, searchTerm, sortColumn, sortOrder, page, pageSize);

            List<SponsorshipsWithEventName> results = new List<SponsorshipsWithEventName>();
            foreach(var e in sponsorship)
            {
                var eventNames = await _eventRepository.GetEventById(e.EventId);
                SponsorshipsWithEventName sponsorshipsWithEvent = new SponsorshipsWithEventName
                {
                    Id = e.Id,
                    Description = e.Description,
                    Type = e.Type,
                    Title = e.Title,
                    Sum = e.Sum,
                    SponsorId = e.SponsorId,
                    EventId = e.EventId,
                    EventName = eventNames.Name
                };

                results.Add(sponsorshipsWithEvent);
            }
            if (results == null)
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
            if (createSponsorshipDTO.Sum <= 0)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The sum value is not valid"
                };
            }
            else if (createSponsorshipDTO.Title.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The title value is not valid"
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

                    var sum = createSponsorshipDTO.Sum;
                    string type;
                    if (sum < 1000000)
                    {
                        type = "Bronze";
                    }
                    else if (1000000 <= sum && sum <= 5000000)
                    {
                        type = "Silver";
                    }
                    else if (5000000 <= sum && sum <= 10000000)
                    {
                        type = "Gold";
                    }
                    else
                    {
                        type = "Platinum";
                    }

                    sponsorshipExist.Type = type;

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
