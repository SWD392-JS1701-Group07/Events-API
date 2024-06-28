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
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public TicketService(ITicketRepository ticketRepository,
            IMapper mapper,
            IAccountRepository accountRepository)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
			_accountRepository = accountRepository;

		}
        public async Task<string> CreateTicket(CreateTicketRequest request)
        {

            return await _ticketRepository.CreateTicket(request);
        }

		public async Task<IEnumerable<TicketDTO>> GetTicketFilter(int accountId = 1, bool? isBought = null, string? orderId = null, string? searchTern = null, string? includeProps = null)
		{
            var accountFromDb = await _accountRepository.GetAccountById(accountId)??throw new KeyNotFoundException("Not found account from DB");
			return _mapper.Map<IEnumerable<TicketDTO>>
				(await _ticketRepository.GetTicketFilter(accountFromDb, isBought, orderId, searchTern, includeProps));
		}
    }
}
