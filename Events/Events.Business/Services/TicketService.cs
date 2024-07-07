
using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Http;

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

		public async Task<BaseResponse> GetTicketById(string ticketId)
		{
			try
			{
				var ticketFromDb = await _ticketRepository.GetTicketById(ticketId);
				var ownerOfEvebt = await _accountRepository.GetAccountById(ticketFromDb.Event.Id) ?? throw new KeyNotFoundException("Owner not found");
				ticketFromDb.Event.Owner = ownerOfEvebt;

				return new BaseResponse
				{
					StatusCode = StatusCodes.Status200OK,
					Message = "Get Ticket detail successfully",
					Data = _mapper.Map<TicketDTO>(ticketFromDb),
				};
			}
			catch (KeyNotFoundException ex)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status404NotFound,
					IsSuccess = false,
					Message = ex.Message
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse
				{
					StatusCode = StatusCodes.Status500InternalServerError,
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<IEnumerable<SimpleTicketDTO>> GetTicketFilter(int accountId = 1, bool? isBought = null, string? orderId = null, string? searchTern = null, string? includeProps = null)
		{
			var accountFromDb = await _accountRepository.GetAccountById(accountId)??throw new KeyNotFoundException("Not found account from DB");
			return _mapper.Map<IEnumerable<SimpleTicketDTO>>
				(await _ticketRepository.GetTicketFilter(accountFromDb, isBought, orderId, searchTern, includeProps));
		}
	}
}