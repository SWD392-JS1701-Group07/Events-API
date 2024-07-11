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
using Microsoft.AspNetCore.Http;
using Events.Utils;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace Events.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly EmailHelper _emailHelper;
        private readonly CloudinaryHelper _cloudinaryHelper;

        public AccountService(IAccountRepository accountRepository, 
            IMapper mapper, 
            ISponsorRepository sponsorRepository, 
            EmailHelper emailHelper, 
            CloudinaryHelper cloudinaryHelper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _sponsorRepository = sponsorRepository;
            _emailHelper = emailHelper;
            _cloudinaryHelper = cloudinaryHelper;
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
            if (!Regex.IsMatch(createAccountDTO.PhoneNumber, RegexBase.PhoneNumberRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The phone number is not in a correct format"
                };
            }
            else if (!Regex.IsMatch(createAccountDTO.Email, RegexBase.GmailRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The email is not in a correct format"
                };
            }
            else if (createAccountDTO.Username.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The username can not be empty"
                };
            }
            else if (createAccountDTO.Name.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The name can not be empty"
                };
            }
            else
            {
                var email = await _accountRepository.GetAccountByEmail(createAccountDTO.Email);
                var phoneNumber = await _accountRepository.GetAccountByPhoneNumber(createAccountDTO.PhoneNumber);
                var username = await _accountRepository.GetAccountByUsername(createAccountDTO.Username);
                var studentId = await _accountRepository.GetAccountByStudentId(createAccountDTO.StudentId);

                if (email != null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "This email is already existed"
                    };
                }
                else if (phoneNumber != null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
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
                } else if(studentId != null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 400,
                        Data = null,
                        IsSuccess = false,
                        Message = "This StudentId is already existed"
                    };
                }
                else
                {
                    string examplePassword;
                    if (createAccountDTO.Password.Trim().IsNullOrEmpty())
                    {
                        examplePassword = GenerateRandomPassword(8);
                    }
                    else
                    {
                        examplePassword = createAccountDTO.Password.Trim();
                    }

                    var account = new AccountDTO
                    {
                        Name = createAccountDTO.Name,
                        Email = createAccountDTO.Email.Trim(),
                        Username = createAccountDTO.Username.Trim(),
                        Password = examplePassword,
                        StudentId = createAccountDTO.StudentId.Trim(),
                        PhoneNumber = createAccountDTO.PhoneNumber.Trim(),
                        Dob = createAccountDTO.Dob,
                        Gender = createAccountDTO.Gender,
                        AvatarUrl = createAccountDTO.AvatarUrl,
                        AccountStatus = "Active",
                        RoleId = createAccountDTO.RoleId,
                        SubjectId = createAccountDTO.SubjectId
                    };

                    _emailHelper.SendEmailToNewAccount(createAccountDTO.Email, createAccountDTO.Username, examplePassword);

                    var accountCreate = await _accountRepository.CreateAccount(_mapper.Map<Account>(account));

                    if (accountCreate != null)
                    {
                        if (accountCreate.RoleId == 3)
                        {
                            Sponsor sponsor = new Sponsor
                            {
                                Name = accountCreate.Name,
                                Email = accountCreate.Email.Trim(),
                                PhoneNumber = accountCreate.PhoneNumber,
                                AvatarUrl = null,
                                AccountId = accountCreate.Id
                            };

                            await _sponsorRepository.AddSponsorAsync(sponsor);
                        }

                        return new BaseResponse
                        {
                            StatusCode = 200,
                            Data = accountCreate,
                            IsSuccess = true,
                            Message = "Created successfully"
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

        public async Task<BaseResponse> GetAccountByRole(int roleId, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var accounts = await _accountRepository.GetAccountByRole(roleId, searchTerm, sortColumn, sortOrder, page, pageSize);

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
            if (registerAccountDTO.Username.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 501,
                    Data = null,
                    IsSuccess = false,
                    Message = "Please fill the username field"
                };
            } 
            else if (registerAccountDTO.Password.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 501,
                    Data = null,
                    IsSuccess = false,
                    Message = "Please fill the password field"
                };
            }
            else if (registerAccountDTO.Email.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 501,
                    Data = null,
                    IsSuccess = false,
                    Message = "Please fill the email field"
                };
            }
            else
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
                        Name = "",
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
        }

        public async Task<BaseResponse> UpdateAccount(int id, UpdateAccountDTO updateAccountDTO)
        {
            if (!Regex.IsMatch(updateAccountDTO.PhoneNumber, RegexBase.PhoneNumberRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The phone number is not in a correct format"
                };
            }
            else if (!Regex.IsMatch(updateAccountDTO.Email, RegexBase.GmailRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The email is not in a correct format"
                };
            }
            else if (updateAccountDTO.Name.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The name can not be empty"
                };
            }

            var account = await _accountRepository.GetAccountById(id);
            var emailExist = await _accountRepository.GetAccountByEmail(updateAccountDTO.Email);
            var phoneExist = await _accountRepository.GetAccountByPhoneNumber(updateAccountDTO.PhoneNumber);
            var studentIdExist = await _accountRepository.GetAccountByStudentId(updateAccountDTO.StudentId);

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
            else if (emailExist != null && emailExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "Email already existed"
                };
            }
            else if (phoneExist != null && phoneExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "Phone number already existed"
                };
            }
            else if (studentIdExist != null && studentIdExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "StudentId already existed"
                };
            }
            else
            {
                account.Name = updateAccountDTO.Name;
                account.Email = updateAccountDTO.Email.Trim();
                if(updateAccountDTO.StudentId != null)
                {
                    account.StudentId = updateAccountDTO.StudentId.Trim();
                }
                else
                {
                    account.StudentId = updateAccountDTO.StudentId;
                }
      
                account.PhoneNumber = updateAccountDTO.PhoneNumber.Trim();
                account.Dob = DateOnly.FromDateTime(updateAccountDTO.Dob);
                account.Gender = Enum.Parse<Gender>(updateAccountDTO.Gender);
                account.AccountStatus = Enum.Parse<AccountStatus>(updateAccountDTO.AccountStatus);
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
                else
                {
                    if (account.RoleId == 3)
                    {
                        var sponsor = await _sponsorRepository.GetSponsorByAccountId(account.Id);
                        sponsor.Name = account.Name;
                        sponsor.PhoneNumber = account.PhoneNumber;
                        sponsor.Email = account.Email;
                        sponsor.AvatarUrl = account.AvatarUrl;
                        await _sponsorRepository.UpdateSponsorAsync(sponsor);
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

        public async Task<BaseResponse> UpdateProfile(int id, UpdateProfile updateProfile, IFormFile avatarFile)
        {
            if (!Regex.IsMatch(updateProfile.PhoneNumber, RegexBase.PhoneNumberRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The phone number is not in a correct format"
                };
            }
            else if (!Regex.IsMatch(updateProfile.Email, RegexBase.GmailRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The email is not in a correct format"
                };
            }
            else if (updateProfile.Name.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The name can not be empty"
                };
            }
            var account = await _accountRepository.GetAccountById(id);
            var emailExist = await _accountRepository.GetAccountByEmail(updateProfile.Email);
            var phoneExist = await _accountRepository.GetAccountByPhoneNumber(updateProfile.PhoneNumber);
            var studentIdExist = await _accountRepository.GetAccountByStudentId(updateProfile.StudentId);

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
            else if (emailExist != null && emailExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "Email already existed"
                };
            }
            else if (phoneExist != null && phoneExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "Phone number already existed"
                };
            }
            else if (studentIdExist != null && studentIdExist.Id != account.Id)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "StudentId already existed"
                };
            }
            else
            {
                account.Name = updateProfile.Name;
                account.Email = updateProfile.Email.Trim();
                account.StudentId = updateProfile.StudentId;
                account.PhoneNumber = updateProfile.PhoneNumber;
                account.Dob = DateOnly.FromDateTime(updateProfile.Dob);
                account.Gender = Enum.Parse<Gender>(updateProfile.Gender);

                if (avatarFile != null)
                {
                    var imageUrl = await _cloudinaryHelper.UploadImageAsync(avatarFile);
                    account.AvatarUrl = imageUrl;
                }

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
                else
                {
                    if (account.RoleId == 3)
                    {
                        var sponsor = await _sponsorRepository.GetSponsorByAccountId(account.Id);
                        sponsor.Name = account.Name;
                        sponsor.PhoneNumber = account.PhoneNumber;
                        sponsor.Email = account.Email;
                        sponsor.AvatarUrl = account.AvatarUrl;
                        await _sponsorRepository.UpdateSponsorAsync(sponsor);
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

        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
