
using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;

namespace Events.Business.Services
{
	public class TicketService : ITicketService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IMapper _mapper;

		public TicketService(ITicketRepository ticketRepository, IAccountRepository accountRepository, ICustomerRepository customerRepository, IMapper mapper)
		{
			_ticketRepository=ticketRepository;
			_accountRepository=accountRepository;
			_customerRepository=customerRepository;
			_mapper=mapper;
		}

		public async Task<BaseResponse> GetTicketById(string ticketId)
		{
			try
			{
				var ticketFromDb = await _ticketRepository.GetTicketById(ticketId);

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

		public async Task<BaseResponse> GetTicketFilter(string email = "john@example.com", bool? isBought = null, string? orderId = null, string? searchTern = null, string? includeProps = null)
		{
			try
			{
				var accountFromDb = await _accountRepository.GetAccountByEmail(email);
				if (accountFromDb == null)
				{
					return new BaseResponse
					{
						StatusCode = StatusCodes.Status404NotFound,
						IsSuccess = false,
						Message = "Account not found!!"
					};
				}
				int? customerId = null;
				if (accountFromDb.RoleId != 1 && accountFromDb.RoleId != 4)
				{
					customerId = (await _customerRepository.GetCustomerByEmail(email)).Id;
				}
				var tickets = await _ticketRepository.GetTicketFilter(accountFromDb, customerId, isBought, orderId, searchTern, includeProps);
				if(!tickets.Any())
				{
					return new BaseResponse
					{
						IsSuccess = false,
						StatusCode = StatusCodes.Status404NotFound,
						Message = "Not found any ticket !!",
					};
				}
				return new BaseResponse
				{
					IsSuccess = true,
					StatusCode = StatusCodes.Status200OK,
					Message = "Get ticket succesfull !!",
					Data = _mapper.Map<IEnumerable<SimpleTicketDTO>>(tickets)
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
	}
}