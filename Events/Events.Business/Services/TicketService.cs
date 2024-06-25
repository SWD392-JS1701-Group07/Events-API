using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;

namespace Events.Business.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;
        public TicketService(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }
        public async Task<string> CreateTicket(CreateTicketRequest request)
        {

            return await _ticketRepository.CreateTicket(request);
        }

		public async Task<IEnumerable<TicketDTO>> GetTicketFilter(bool isBought = false ,string? orderId = null, string? includeProps = null)
		{
            return _mapper.Map<IEnumerable<TicketDTO>>
				(await _ticketRepository.GetTicketFilter(isBought, orderId, includeProps));
		}

		public async Task UpdateOrderStatus(string orderId, string? responeCode)
        {
            await _ticketRepository.UpdateOrderStatus(orderId, responeCode);
        }
    }
}
