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

namespace Events.Business.Services
{
    public class SponsorService : ISponsorService
	{
		private readonly ISponsorRepository _sponsorRepository;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _environment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public SponsorService(
			ISponsorRepository sponsorRepository,
			IMapper mapper, 
			IWebHostEnvironment environment,
			IHttpContextAccessor httpContextAccessor
			)
        {
            _sponsorRepository = sponsorRepository;
			_mapper = mapper;
			_environment = environment;
			_httpContextAccessor = httpContextAccessor;
		}
        public async Task<BaseResponse> AddSponsorAsync(CreateSponsorDTO sponsorDto)
		{
			var sponsor = _mapper.Map<Sponsor>(sponsorDto);

			if (sponsorDto.AvatarFile != null && sponsorDto.AvatarFile.Length > 0)
			{
				sponsor.AvatarUrl = await ImageHelper.SaveImageAsync(sponsorDto.AvatarFile, _environment);
			}

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
			if(createdSponsorDto.AvatarUrl is not null)
			{
				createdSponsorDto.AvatarUrl = $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://" +
											$"{_httpContextAccessor?.HttpContext?.Request.Host}" +
											$"{_httpContextAccessor?.HttpContext?.Request.PathBase}" +
											$"{createdSponsorDto.AvatarUrl}";
			}

			return new BaseResponse
			{
				StatusCode = 201,
				IsSuccess = true,
				Data = createdSponsorDto
			};
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
				if (!string.IsNullOrWhiteSpace(sponsor.AvatarUrl))
				{
					ImageHelper.DeleteImage(sponsor.AvatarUrl, _environment);
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
			foreach(var sponsorDTO in sponsorListDTO)
			{
				if(sponsorDTO.AvatarUrl is not null)
				sponsorDTO.AvatarUrl = $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://" +
											$"{_httpContextAccessor?.HttpContext?.Request.Host}" +
											$"{_httpContextAccessor?.HttpContext?.Request.PathBase}" +
											$"{sponsorDTO.AvatarUrl}";
			}
			
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
				if (sponsorDTO.AvatarUrl is not null)
					sponsorDTO.AvatarUrl = $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://" +
													$"{_httpContextAccessor?.HttpContext?.Request.Host}" +
													$"{_httpContextAccessor?.HttpContext?.Request.PathBase}" +
													$"{sponsorDTO.AvatarUrl}";
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
				if (updateSponsor.AvatarFile != null && updateSponsor.AvatarFile.Length > 0)
				{
					if (!string.IsNullOrEmpty(sponsor.AvatarUrl))
					{
						ImageHelper.DeleteImage(sponsor.AvatarUrl, _environment);
					}
					sponsor.AvatarUrl = await ImageHelper.SaveImageAsync(updateSponsor.AvatarFile, _environment);
				}
				_mapper.Map(updateSponsor, sponsor);
				var result = await _sponsorRepository.UpdateSponsorAsync(sponsor);
				var sponsorDTO = _mapper.Map<SponsorDTO>(sponsor);
				if (sponsorDTO.AvatarUrl is not null)
				{
					sponsorDTO.AvatarUrl = $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://" +
												$"{_httpContextAccessor?.HttpContext?.Request.Host}" +
												$"{_httpContextAccessor?.HttpContext?.Request.PathBase}" +
												$"{sponsorDTO.AvatarUrl}";
				}
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
