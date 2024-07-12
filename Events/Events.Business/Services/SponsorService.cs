using AutoMapper;
using Azure.Core;
using Events.Utils.Helper;
using Events.Business.Services.Interfaces;
using Events.Data.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Events.Data.Repositories;
using Events.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models.DTOs.Request;
using Events.Models.DTOs;
using Events.Utils.Helpers;
using Events.Utils;
using System.Text.RegularExpressions;
using static QRCoder.PayloadGenerator;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace Events.Business.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly CloudinaryHelper _cloudinaryHelper;
        private readonly EmailHelper _emailHelper;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            IAccountRepository accountRepository,
            IMapper mapper,
            CloudinaryHelper cloudinaryHelper,
            EmailHelper emailHelper
            )
        {
            _sponsorRepository = sponsorRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _cloudinaryHelper = cloudinaryHelper;
            _emailHelper = emailHelper;
        }
        public async Task<BaseResponse> AddSponsorAsync(CreateSponsorDTO sponsorDto)
        {
            if (!Regex.IsMatch(sponsorDto.PhoneNumber, RegexBase.PhoneNumberRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The phone number is not in a correct format"
                };
            }
            else if (!Regex.IsMatch(sponsorDto.Email, RegexBase.GmailRegex))
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The email is not in a correct format"
                };
            }
            else if (sponsorDto.Name.Trim() == null)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "The username can not be empty"
                };
            }

            var emailExist = await _sponsorRepository.GetSponsorByEmailAsync(sponsorDto.Email);
            var phoneExist = await _sponsorRepository.GetSponsorByPhoneNumberAsync(sponsorDto.PhoneNumber);

            if (emailExist != null)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "This email is already existed"
                };
            }
            else if (phoneExist != null)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    Data = null,
                    IsSuccess = false,
                    Message = "This phone number is already existed"
                };
            }
            else
            {
                if (sponsorDto.CreateAccount)
                {
                    var accountEmailExist = await _accountRepository.GetAccountByEmail(sponsorDto.Email);
                    var accountPhoneExist = await _accountRepository.GetAccountByPhoneNumber(sponsorDto.PhoneNumber);
                    var userNameExist = await _accountRepository.GetAccountByUsername(sponsorDto.Email);

                    if (accountEmailExist != null)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This email is already existed in account"
                        };
                    }
                    else if (accountPhoneExist != null)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This phone number is already existed in account"
                        };
                    }
                    else if (userNameExist != null)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This username is already existed in account"
                        };
                    }
                    else
                    {
                        Account account = new Account
                        {
                            Name = sponsorDto.Name,
                            Email = sponsorDto.Email,
                            PhoneNumber = sponsorDto.PhoneNumber,
                            Username = sponsorDto.Email,
                            Password = "12345678",
                            Dob = DateOnly.FromDateTime(DateTime.Now),
                            RoleId = 3,
                            AccountStatus = Enums.AccountStatus.Active,
                            Gender = Enums.Gender.Others,
                        };

                        var resultAccount = await _accountRepository.CreateAccount(account);

                        if (resultAccount == null)
                        {
                            return new BaseResponse
                            {
                                StatusCode = 500,
                                Data = null,
                                IsSuccess = false,
                                Message = "Create sponsor failed because of account"
                            };
                        }
                        else
                        {

                            _emailHelper.SendEmailToNewAccount(resultAccount.Email, resultAccount.Username, resultAccount.Password);
                            Sponsor sponsor = new Sponsor
                            {
                                Email = sponsorDto.Email,
                                PhoneNumber = sponsorDto.PhoneNumber,
                                Name = sponsorDto.Name,
                                AvatarUrl = null,
                                AccountId = resultAccount.Id,
                            };

                            var result = await _sponsorRepository.AddSponsorAsync(sponsor);

                            if (!result)
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 500,
                                    Message = "Failed to add sponsor",
                                    IsSuccess = false
                                };
                            }

                            var createdSponsorDto = _mapper.Map<SponsorDTO>(sponsor);
                            return new BaseResponse
                            {
                                StatusCode = 201,
                                IsSuccess = true,
                                Data = createdSponsorDto
                            };
                        }
                    }
                }
                else
                {
                    Sponsor sponsor = new Sponsor
                    {
                        Email = sponsorDto.Email,
                        PhoneNumber = sponsorDto.PhoneNumber,
                        Name = sponsorDto.Name,
                        AvatarUrl = null,
                        AccountId = null,
                    };

                    var result = await _sponsorRepository.AddSponsorAsync(sponsor);

                    if (!result)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Message = "Failed to add sponsor",
                            IsSuccess = false
                        };
                    }

                    var createdSponsorDto = _mapper.Map<SponsorDTO>(sponsor);
                    return new BaseResponse
                    {
                        StatusCode = 201,
                        IsSuccess = true,
                        Data = createdSponsorDto
                    };
                }
            }
        }

        public async Task<BaseResponse> DeleteSponsorAsync(int id)
        {
            try
            {
                var sponsor = await _sponsorRepository.GetSponsorByIdAsync(id);
                if (sponsor == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Sponsor not found",
                        IsSuccess = false
                    };
                }

                var result = await _sponsorRepository.DeleteSponsorAsync(sponsor);
                if (!result)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Message = "Failed to delete sponsor",
                        IsSuccess = false
                    };
                }
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Sponsor deleted successfully",
                    IsSuccess = true
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<BaseResponse> GetAllSponsor(string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var sponsorListDTO = _mapper.Map<IEnumerable<SponsorDTO>>(
                                await _sponsorRepository.GetAllSponsor(searchTerm, sortColumn, sortOrder, page, pageSize));

            return !sponsorListDTO.Any()
                ? new BaseResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    IsSuccess = false,
                    Message = "Not found any sponsor"
                }
                : new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = sponsorListDTO,
                    IsSuccess = true
                };
        }

        public async Task<BaseResponse> GetSponsorByIdAsync(int id)
        {
            try
            {
                var sponsor = await _sponsorRepository.GetSponsorByIdAsync(id);
                if (sponsor == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Sponsor not found",
                        IsSuccess = false
                    };
                }

                var sponsorDTO = _mapper.Map<SponsorDTO>(sponsor);
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    Data = sponsorDTO
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    IsSuccess = false
                };

            }
        }

        public async Task<BaseResponse> UpdateSponsorAsync(int id, UpdateSponsorDTO updateSponsor)
        {
            try
            {
                if (!Regex.IsMatch(updateSponsor.PhoneNumber, RegexBase.PhoneNumberRegex))
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "The phone number is not in a correct format"
                    };
                }
                else if (!Regex.IsMatch(updateSponsor.Email, RegexBase.GmailRegex))
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "The email is not in a correct format"
                    };
                }
                else if (updateSponsor.Name.Trim() == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        Data = null,
                        IsSuccess = false,
                        Message = "The username can not be empty"
                    };
                }

                var sponsor = await _sponsorRepository.GetSponsorByIdAsync(id);
                if (sponsor == null)
                {
                    return new BaseResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        Message = "Sponsor not found"
                    };
                }

                else
                {
                    var emailExist = await _sponsorRepository.GetSponsorByEmailAsync(updateSponsor.Email);
                    var phoneExist = await _sponsorRepository.GetSponsorByPhoneNumberAsync(updateSponsor.PhoneNumber);

                    if (emailExist != null && emailExist.Id != sponsor.Id)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This email is already existed"
                        };
                    }
                    else if (phoneExist != null && phoneExist.Id != sponsor.Id)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This phone number is already existed"
                        };
                    }
                    else
                    {
                        sponsor.Name = updateSponsor.Name;
                        sponsor.Email = updateSponsor.Email;
                        sponsor.PhoneNumber = updateSponsor.PhoneNumber;

                            if (updateSponsor.AvatarFile != null)
                        {
                            if (sponsor.AvatarUrl != null)
                            {
                                var imageExist = await _cloudinaryHelper.ImageExistsAsync(sponsor.AvatarUrl);

                                if (imageExist)
                                {
                                    var deleteImage = await _cloudinaryHelper.DeleteImageAsync(sponsor.AvatarUrl);

                                    if (deleteImage)
                                    {
                                        var imageUrl = await _cloudinaryHelper.UploadImageAsync(updateSponsor.AvatarFile);
                                        sponsor.AvatarUrl = imageUrl;
                                    }
                                }
                                else
                                {
                                    var imageUrl = await _cloudinaryHelper.UploadImageAsync(updateSponsor.AvatarFile);
                                    sponsor.AvatarUrl = imageUrl;
                                }
                            }
                            else
                            {
                                var imageUrl = await _cloudinaryHelper.UploadImageAsync(updateSponsor.AvatarFile);
                                sponsor.AvatarUrl = imageUrl;
                            }
                        }

                        if (sponsor.AccountId != null)
                        {
                            var sponsorAccount = await _accountRepository.GetAccountById((int)sponsor.AccountId);
                            var accountEmailExist = await _accountRepository.GetAccountByEmail(sponsor.Email);
                            var phoneNumberEmailExist = await _accountRepository.GetAccountByPhoneNumber(sponsor.PhoneNumber);

                            if (accountEmailExist != null && accountEmailExist.Id != sponsorAccount.Id)
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 500,
                                    Message = "Email already existed in account",
                                    IsSuccess = false
                                };
                            }
                            else if (phoneNumberEmailExist != null && phoneNumberEmailExist.Id != sponsorAccount.Id)
                            {
                                return new BaseResponse
                                {
                                    StatusCode = 500,
                                    Message = "Email already existed in account",
                                    IsSuccess = false
                                };
                            }
                            else
                            {
                                sponsorAccount.Email = sponsor.Email;
                                sponsorAccount.PhoneNumber = sponsor.PhoneNumber;
                                sponsorAccount.AvatarUrl = sponsor.AvatarUrl;

                                var accountSponsorUpdate = await _accountRepository.UpdateAccount(sponsorAccount);

                                if (accountSponsorUpdate)
                                {
                                    var result = await _sponsorRepository.UpdateSponsorAsync(sponsor);

                                    if (result)
                                    {
                                        return new BaseResponse
                                        {
                                            StatusCode = 200,
                                            Message = "Update successfully sponsor",
                                            IsSuccess = true,
                                            Data = sponsor
                                        };
                                    }
                                    else
                                    {
                                        return new BaseResponse
                                        {
                                            StatusCode = 500,
                                            Message = "Failed to update sponsor",
                                            IsSuccess = false
                                        };
                                    }
                                }
                                else
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 500,
                                        Message = "Failed to update sponsor because of account entity",
                                        IsSuccess = false
                                    };
                                }
                            }
                        }
                        else
                        {
                            if (updateSponsor.CreateAccount)
                            {
                                var accountEmailExist = await _accountRepository.GetAccountByEmail(sponsor.Email);
                                var phoneNumberEmailExist = await _accountRepository.GetAccountByPhoneNumber(sponsor.PhoneNumber);
                                var userNameExist = await _accountRepository.GetAccountByUsername(sponsor.Email);
                                if (accountEmailExist != null)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 500,
                                        Message = "Email already existed in account",
                                        IsSuccess = false
                                    };
                                }
                                else if (phoneNumberEmailExist != null)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 500,
                                        Message = "Email already existed in account",
                                        IsSuccess = false
                                    };
                                }
                                else if (userNameExist != null)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 500,
                                        Message = "Username already existed in account",
                                        IsSuccess = false
                                    };
                                }
                                else
                                {
                                    Account account = new Account
                                    {
                                        Name = sponsor.Name,
                                        Email = sponsor.Email,
                                        PhoneNumber = sponsor.PhoneNumber,
                                        Username = sponsor.Email,
                                        Password = "12345678",
                                        AvatarUrl = sponsor.AvatarUrl,
                                        Dob = DateOnly.FromDateTime(DateTime.Now),
                                        RoleId = 3,
                                        AccountStatus = Enums.AccountStatus.Active,
                                        Gender = Enums.Gender.Others,
                                    };

                                    var resultAccount = await _accountRepository.CreateAccount(account);

                                    if (resultAccount == null)
                                    {
                                        return new BaseResponse
                                        {
                                            StatusCode = 500,
                                            Data = null,
                                            IsSuccess = false,
                                            Message = "Create sponsor failed because of account"
                                        };
                                    }
                                    else
                                    {
                                        _emailHelper.SendEmailToNewAccount(resultAccount.Email, resultAccount.Username, resultAccount.Password);
                                        sponsor.AccountId = resultAccount.Id;
                                        var result = await _sponsorRepository.UpdateSponsorAsync(sponsor);
                                        if (result)
                                        {
                                            return new BaseResponse
                                            {
                                                StatusCode = 200,
                                                IsSuccess = true,
                                                Message = "Updated succesfully"
                                            };
                                        }
                                        else
                                        {
                                            return new BaseResponse
                                            {
                                                StatusCode = 500,
                                                Message = "Failed to update sponsor",
                                                IsSuccess = false
                                            };
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var result = await _sponsorRepository.UpdateSponsorAsync(sponsor);
                                if (result)
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 200,
                                        IsSuccess = true,
                                        Message = "Updated succesfully"
                                    };
                                }
                                else
                                {
                                    return new BaseResponse
                                    {
                                        StatusCode = 500,
                                        Message = "Failed to update sponsor",
                                        IsSuccess = false
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    IsSuccess = false
                };
            }

        }

        public async Task<Sponsor> GetSponsorByEmailAsync(string email)
        {
            return await _sponsorRepository.GetSponsorByEmailAsync(email);
        }
    }
}
