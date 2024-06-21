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
using Events.Utils.Helpers;

namespace Events.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly EmailHelper _emailHelper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, ISponsorRepository sponsorRepository, EmailHelper emailHelper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _sponsorRepository = sponsorRepository;
            _emailHelper = emailHelper;
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
                string examplePassword;
                if(createAccountDTO.Password == null)
                {
                    examplePassword = GenerateRandomPassword(8);
                }
                else
                {
                    examplePassword = createAccountDTO.Password;
                }

                var account = new AccountDTO
                {
                    Name = createAccountDTO.Name,
                    Email = createAccountDTO.Email,
                    Username = createAccountDTO.Username,
                    Password = examplePassword,
                    StudentId = createAccountDTO.StudentId,
                    PhoneNumber = createAccountDTO.PhoneNumber,
                    Dob = createAccountDTO.Dob,
                    Gender = createAccountDTO.Gender,
                    AvatarUrl = createAccountDTO.AvatarUrl,
                    AccountStatus = "Active",
                    RoleId = createAccountDTO.RoleId,
                    SubjectId = createAccountDTO.SubjectId
                };

                _emailHelper.SendEmailToNewAccount(createAccountDTO.Email, createAccountDTO.Username, examplePassword);

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

        public async Task<BaseResponse> RegisterAccount(RegisterAccountDTO registerAccountDTO)
        {
            var emailExist = await _accountRepository.GetAccountByEmail(registerAccountDTO.Email);
            var usernameExist = _accountRepository.GetAccountByUsername(registerAccountDTO.Username);
            if (emailExist != null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "This email is already existed"
                };
            }
            else if (usernameExist == null)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "This username is already existed"
                };
            }
            else if (registerAccountDTO.Password != registerAccountDTO.ConfirmPassword)
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Data = null,
                    IsSuccess = false,
                    Message = "Password is not the same"
                };
            }
            else
            {
                AccountDTO account = new AccountDTO
                {
                    Email = registerAccountDTO.Email,
                    Name = "John",
                    Username = registerAccountDTO.Username,
                    Password = registerAccountDTO.Password,
                    Dob = DateTime.UtcNow,
                    Gender = "Others",
                    AccountStatus = "Active",
                    RoleId = 2
                };

                var result = await _accountRepository.RegisterAccount(_mapper.Map<Account>(account));

                if (result)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        Data = account,
                        IsSuccess = true,
                        Message = "Create successfully"
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

        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
