using AutoMapper;
using Events.Data;
using Events.Data.DTOs;
using Events.Data.DTOs.Request;
using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Mapping
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
