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

namespace Events.Business.Services
{
    public class SponsorService : ISponsorService
	{
		private readonly ISponsorRepository _sponsorRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly CloudinaryHelper _cloudinaryHelper;
		public SponsorService(
			ISponsorRepository sponsorRepository,
			IAccountRepository accountRepository,
			IMapper mapper,
			CloudinaryHelper cloudinaryHelper
			)
        {
            _sponsorRepository = sponsorRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _cloudinaryHelper = cloudinaryHelper;
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
				if(sponsorDto.CreateAccount)
				{
					var accountEmailExist = await _accountRepository.GetAccountByEmail(sponsorDto.Email);
					var accountPhonelExist = await _accountRepository.GetAccountByPhoneNumber(sponsorDto.PhoneNumber);

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
                    else if (accountPhonelExist != null)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            Data = null,
                            IsSuccess = false,
                            Message = "This phone number is already existed in account"
                        };
                    }
					else{
                        Account account = new Account
                        {
							Name = sponsorDto.Name,
							Email = sponsorDto.Email,
							PhoneNumber = sponsorDto.PhoneNumber,
							Username = sponsorDto.Name,
							Password = "123",
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
                            Sponsor sponsor = new Sponsor
                            {
								Email = sponsorDto.Email,
								PhoneNumber= sponsorDto.PhoneNumber,
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

		public async Task<BaseResponse> GetAllSponsor()
		{
			var sponsorListDTO = _mapper.Map<IEnumerable<SponsorDTO>>(
								await _sponsorRepository.GetAllSponsor());
			
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
				var sponsor = await _sponsorRepository.GetSponsorByIdAsync(id);
				if (sponsor==null)
				{
					return new BaseResponse
					{
						StatusCode = StatusCodes.Status404NotFound,
						IsSuccess=false,
						Message = "Sponsor not found"
					};
				}
				_mapper.Map(updateSponsor, sponsor);
				var result = await _sponsorRepository.UpdateSponsorAsync(sponsor);
				var sponsorDTO = _mapper.Map<SponsorDTO>(sponsor);

				if (!result)
				{
					return new BaseResponse
					{
						StatusCode = 500,
						Message = "Failed to update sponsor",
						IsSuccess = false
					};
				}
				return new BaseResponse
				{
					StatusCode = 200,
					IsSuccess = true,
					Data = sponsorDTO
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

        public async Task<Sponsor> GetSponsorByEmailAsync(string email)
        {
            return await _sponsorRepository.GetSponsorByEmailAsync(email);
        }
    }
}
