using Events.Business.Services.Interfaces;
using Events.Models.Models;
using Events.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models.DTOs;
using AutoMapper;
using Events.Models.DTOs.Response;

namespace Events.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        public async Task<AccountDTO> CheckLogin(string username, string password)
        {
            var account = await _accountRepository.GetAccount(username, password);
            return _mapper.Map<AccountDTO>(account);
        }

        public async Task<BaseResponse> GetAccountById(int id)
        {
            var account = await _accountRepository.GetAccountById(id);

            if(account == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can't found this account"
                };
            }
            else
            {
                var results = _mapper.Map<AccountDTO>(account);
                return new BaseResponse
                {
                    StatusCode = 200,
                    Data = results,
                    IsSuccess = true,
                    Message = "Return successfully"
                };
            }
        }

        public async Task<BaseResponse> GetAllAccounts()
        {
            var accounts = await _accountRepository.GetAllAccounts();

            var results = _mapper.Map<List<AccountDTO>>(accounts);
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
    }
}
