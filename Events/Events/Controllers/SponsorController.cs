using Events.Business.Services;
using Events.Data.DTOs.Request;
using Events.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/sponsors")]
    [ApiVersionNeutral]

    public class SponsorController : ControllerBase
	{
		private readonly ISponsorService _sponsorService;
        public SponsorController(ISponsorService sponsorService)
        {
            _sponsorService = sponsorService;
        }
		[HttpGet]
		public async Task<IActionResult> GetAllCollaborators()
		{
			var response = await _sponsorService.GetAllSponsor();
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}", Name = nameof(GetSponsor))]
		public async Task<IActionResult> GetSponsor([FromRoute] int id)
		{
			var response = await _sponsorService.GetSponsorByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		public async Task<IActionResult> AddSponsor([FromForm] CreateSponsorDTO sponsorDto)
		{
			if(sponsorDto == null || !ModelState.IsValid) {
				return BadRequest(new BaseResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					IsSuccess = false,
					Message = "Invalid request body"
				});
			}
			var response = await _sponsorService.AddSponsorAsync(sponsorDto);

			if (response.IsSuccess)
			{
				var createdSponsor = response.Data as SponsorDTO;
				return CreatedAtRoute(nameof(GetSponsor), new { id = createdSponsor?.Id }, createdSponsor);
			}

			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateSponsor([FromRoute] int id, [FromForm] UpdateSponsorDTO sponsorDTO)
		{
			if(sponsorDTO == null || !ModelState.IsValid)
			{
				return BadRequest(new BaseResponse
				{
					StatusCode = StatusCodes.Status400BadRequest,
					IsSuccess = false,
					Message = "Invalid request body"
				});
			}
			var response = await _sponsorService.UpdateSponsorAsync(id, sponsorDTO);
			return StatusCode(response.StatusCode, response);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSponsor([FromRoute] int id)
		{
			var reponse = await _sponsorService.DeleteSponsorAsync(id);
			return StatusCode(reponse.StatusCode, reponse);
		}

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetSponsorByEmail(string email)
        {
            var sponsor = await _sponsorService.GetSponsorByEmailAsync(email);

            if (sponsor == null)
            {
                return NotFound(new { Message = "Sponsor not found" });
            }

            return Ok(sponsor);
        }
    } 
}
