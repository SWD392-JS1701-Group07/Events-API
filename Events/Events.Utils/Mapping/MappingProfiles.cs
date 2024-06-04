using AutoMapper;

using Events.Data.DTOs.Request;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using System;


namespace Events.Utils.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Event, EventDTO>()
                .ForMember(d => d.EventStatus, o => o.MapFrom(src => src.EventStatus.ToString()))
                .ReverseMap();

            CreateMap<CreateEventDTO, Event>()
               .ForMember(d => d.EventStatus, o => o.MapFrom(src => src.EventStatus.ToString()));

            CreateMap<Collaborator, CollaboratorDTO>().ReverseMap();
			CreateMap<CreateSponsorDTO, Sponsor>()
			    .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());
			CreateMap<UpdateSponsorDTO, Sponsor>()
				.ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

			CreateMap<SponsorDTO, Sponsor>().ReverseMap();
                
		}

    }
}
