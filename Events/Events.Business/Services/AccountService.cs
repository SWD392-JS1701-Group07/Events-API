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
using Events.Models.DTOs.Request;
using System.Diagnostics;

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

        public async Task<BaseResponse> CreateAccount(CreateAccountDTO createAccountDTO)
        {
            var email = await _accountRepository.GetAccountByEmail(createAccountDTO.Email);
            var phoneNumber = await _accountRepository.GetAccountByPhoneNumber(createAccountDTO.PhoneNumber);
            var username = await _accountRepository.GetAccountByUsername(createAccountDTO.Username);

            if (email != null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "This email is already existed"
                };
            }
            else if (phoneNumber != null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "This phone number is already existed"
                };
            }
            else if (username != null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "This username is already existed"
                };
            }
            else
            {
                var account = new AccountDTO
                {
                    Name = createAccountDTO.Name,
                    Email = createAccountDTO.Email,
                    Username = createAccountDTO.Username,
                    Password = createAccountDTO.Password,
                    StudentId = createAccountDTO.StudentId,
                    PhoneNumber = createAccountDTO.PhoneNumber,
                    Dob = createAccountDTO.Dob,
                    Gender = createAccountDTO.Gender,
                    AvatarUrl = createAccountDTO.AvatarUrl,
                    AccountStatus = "Active",
                    RoleId = createAccountDTO.RoleId,
                    SubjectId = createAccountDTO.SubjectId
                };

                var accountCreate =  await _accountRepository.CreateAccount(_mapper.Map<Account>(account));

                if(accountCreate)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        Data = account,
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

        public async Task<BaseResponse> GetAccountById(int id)
        {
            var account = await _accountRepository.GetAccountById(id);

            if (account == null)
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
