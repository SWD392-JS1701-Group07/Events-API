using AutoMapper;

using Events.Data.DTOs.Request;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
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

            CreateMap<CreateEventDTO, Event>().ForMember(d => d.EventStatus, o => o.Ignore());

            CreateMap<EventSchedule, EventScheduleDTO>().ReverseMap();
            CreateMap<CreateEventScheduleDTO, EventSchedule>();

            CreateMap<Collaborator, CollaboratorDTO>().ReverseMap();  
            CreateMap<CreateSponsorDTO, Sponsor>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

            CreateMap<CreateSponsorEventDTO, Sponsor>()
           .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));
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

            CreateMap<Ticket, TicketDTO>()
                .ForMember(dest => dest.IsCheckIn, o => o.MapFrom(src => src.IsCheckIn.ToString()));

            CreateMap<TicketDTO, Ticket>()
                .ForMember(dest => dest.IsCheckIn, o => o.MapFrom(src => Enum.Parse<IsCheckin>(src.IsCheckIn)));

            CreateMap<Order, OrderDTO>()
                //      .ForMember(dest => dest.PaymentMethod, o => o.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.OrderStatus, o => o.MapFrom(src => src.OrderStatus.ToString()));

            CreateMap<OrderDTO, Order>()
         //       .ForMember(dest => dest.PaymentMethod, o => o.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod)))
                .ForMember(dest => dest.OrderStatus, o => o.MapFrom(src => Enum.Parse<OrderStatus>(src.OrderStatus)))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.OrderDate)));

            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<UpdateCustomerDTO, Customer>();

            CreateMap<Sponsorship, SponsorshipDTO>().ReverseMap();

            CreateMap<CreateSponsorshipEventDTO, Sponsorship>()
                .ForMember(dest => dest.EventId, opt => opt.Ignore())
                .ForMember(dest => dest.SponsorId, opt => opt.Ignore());

            CreateMap<CreateSponsorshipDTO, Sponsorship>()
                .ForMember(dest => dest.EventId, opt => opt.Ignore())
                .ForMember(dest => dest.SponsorId, opt => opt.Ignore());

            CreateMap<Ticket, TicketDTO>()
                .ForMember(dest => dest.EventName, otp => otp.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.EventSchedules, otp => otp.MapFrom(src => src.Event.EventSchedules));

            CreateMap<Subject, SubjectDTO>().ReverseMap();

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.Notes, otp => otp.MapFrom(src => src.OrderNotes))
                .ForMember(dest => dest.TotalPrice, otp => otp.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Tickets, otp => otp.Ignore());

            CreateMap<TicketDetail, Ticket>();

            CreateMap<PaymentResponseModel, Transaction>()
                .ForMember(dest => dest.Description, otp => otp.MapFrom(src => src.OrderDescription))
                .ForMember(dest => dest.RefId, otp => otp.MapFrom(src => src.RefId))
                .ForMember(dest => dest.TransactionDate, otp => otp.MapFrom(src => src.PayDate))
                .ForMember(dest => dest.PaymentMethod, otp => otp.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.VnPayTransactioId, otp => otp.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.VnPayTransactioId, otp => otp.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.ResponseCode, otp => otp.MapFrom(src => src.VnPayResponseCode));

            CreateMap<Ticket, QrCodeDTO>()
                .ForMember(dest => dest.TicketId, otp => otp.MapFrom(src => src.Id))
                .ForMember(dest => dest.EventName, otp => otp.Ignore());

		}

    }
}
    