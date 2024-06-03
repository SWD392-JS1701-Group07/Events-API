using Events.Business.Interfaces;
using Events.Business.Services;
using Events.Data.DTOs.Request;
using Events.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
	[Route("api/[controller]")]
	[ApiVersion("2.0")]
	[ApiExplorerSettings(GroupName = "v2")]
	[ApiController]
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
			var response = await _sponsorService.UpdateSponsorAsync(id, sponsorDTO);
			return StatusCode(response.StatusCode, response);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSponsor([FromRoute] int id)
		{
			var reponse = await _sponsorService.DeleteSponsorAsync(id);
			return StatusCode(reponse.StatusCode, reponse);
		}
	} 
}
