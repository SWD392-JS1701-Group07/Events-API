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
using Events.Utils.Helper;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using static Events.Utils.Enums;

namespace Events.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<BaseResponse> BanAccount(int id)
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
                var result = await _accountRepository.BanAccount(id);

                if(result != false)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        Data = null,
                        IsSuccess = true,
                        Message = "Ban account successfully"
                    };
                }
                else
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "This account is already banned"
                    };
                }
            }
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

        public async Task<BaseResponse> GetAccountByRole(int roleId)
        {
            var accounts = await _accountRepository.GetAccountByRole(roleId);

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

        public async Task<BaseResponse> UpdateAccount(int id, UpdateAccountDTO updateAccountDTO)
        {
            var account = await _accountRepository.GetAccountById(id);

            if (account == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can;t found this account"
                };
            }
            else
            {
                account.Name = updateAccountDTO.Name;
                account.Email = updateAccountDTO.Email;
                account.StudentId = updateAccountDTO.StudentId;
                account.PhoneNumber = updateAccountDTO.PhoneNumber;
                account.Dob = DateOnly.FromDateTime(updateAccountDTO.Dob);
                account.Gender = Enum.Parse<Gender>(updateAccountDTO.Gender);
                account.AccountStatus = Enum.Parse<AccountStatus>(updateAccountDTO.AccountStatus);
                account.RoleId = updateAccountDTO.RoleId;
                account.AvatarUrl = updateAccountDTO.AvatarUrl;
                account.SubjectId = updateAccountDTO.SubjectId;

                var result = await _accountRepository.UpdateAccount(account);

                if (!result)
                {
                    return new BaseResponse 
                    {
                        StatusCode = 500,
                        Message = "Failed to update account",
                        IsSuccess = false
                    };
                }
                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = _mapper.Map<AccountDTO>(account)
                };
            }
        }

        public async Task<BaseResponse> UpdateProfile(int id, UpdateProfile updateProfile)
        {
            var account = await _accountRepository.GetAccountById(id);

            if (account == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Data = null,
                    IsSuccess = false,
                    Message = "Can;t found this account"
                };
            }
            else
            {
                account.Name = updateProfile.Name;
                account.Email = updateProfile.Email;
                account.StudentId = updateProfile.StudentId;
                account.PhoneNumber = updateProfile.PhoneNumber;
                account.Dob = DateOnly.FromDateTime(updateProfile.Dob);
                account.Gender = Enum.Parse<Gender>(updateProfile.Gender);
                account.AvatarUrl = updateProfile.AvatarUrl;
                account.SubjectId = updateProfile.SubjectId;

                var result = await _accountRepository.UpdateAccount(account);

                if (!result)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Message = "Failed to update account",
                        IsSuccess = false
                    };
                }
                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = _mapper.Map<AccountDTO>(account)
                };
            }
        }
    }
}
