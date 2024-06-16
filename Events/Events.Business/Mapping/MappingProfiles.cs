using AutoMapper;

using Events.Data.DTOs.Request;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using System;
using static Events.Utils.Enums;


namespace Events.Business.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Event, EventDTO>()
            .ForMember(d => d.EventStatus, o => o.MapFrom(src => src.EventStatus.ToString()))
            .ReverseMap()
            .ForMember(d => d.EventStatus, o => o.MapFrom(src => Enum.Parse<EventStatus>(src.EventStatus)));

            CreateMap<CreateEventDTO, Event>()
                .ForMember(d => d.EventStatus, o => o.MapFrom(src => Enum.Parse<EventStatus>(src.EventStatus)));

            CreateMap<EventSchedule, EventScheduleDTO>().ReverseMap();
            CreateMap<CreateEventScheduleDTO, EventSchedule>();

            CreateMap<Collaborator, CollaboratorDTO>().ReverseMap();  
            CreateMap<CreateSponsorDTO, Sponsor>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());
            CreateMap<UpdateSponsorDTO, Sponsor>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

            CreateMap<SponsorDTO, Sponsor>().ReverseMap();
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.AccountStatus, o => o.MapFrom(src => src.AccountStatus.ToString()))
                .ForMember(dest => dest.Gender, o => o.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToDateTime(TimeOnly.MinValue)));
        
            CreateMap<AccountDTO, Account>()
                .ForMember(dest => dest.AccountStatus, o => o.MapFrom(src => Enum.Parse<AccountStatus>(src.AccountStatus)))
                .ForMember(dest => dest.Gender, o => o.MapFrom(src => Enum.Parse<Gender>(src.Gender)))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Dob)));

            CreateMap<Sponsorship, SponsorshipDTO>().ReverseMap();

            CreateMap<CreateSponsorshipEventDTO, Sponsorship>()
           .ForMember(dest => dest.EventId, opt => opt.Ignore())
           .ForMember(dest => dest.SponsorId, opt => opt.Ignore());

            CreateMap<CreateSponsorshipDTO, Sponsorship>()
           .ForMember(dest => dest.EventId, opt => opt.Ignore())
           .ForMember(dest => dest.SponsorId, opt => opt.Ignore());
        }

    }
}
