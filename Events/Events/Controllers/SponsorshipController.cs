using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/sponsorships")]
    [ApiVersionNeutral]
    public class SponsorshipController : Controller
    {
        private readonly ISponsorshipService _sponsorshipService;

        public SponsorshipController(ISponsorshipService sponsorshipService)
        {
            _sponsorshipService = sponsorshipService;
        }

        [HttpGet]
        [Authorize(Roles = "1,3,4,5")]
        public async Task<IActionResult> GetAllSponsorship([FromQuery] string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var result = await _sponsorshipService.GetAllSponsorship(searchTerm, sortColumn, sortOrder, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1,3,4,5")]
        public async Task<IActionResult> GetSponsorshipById(int id)
        {
            var result = await _sponsorshipService.GetSponsorshipById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("sponsors/{id}")]
        [Authorize(Roles = "1,3,4,5")]
        public async Task<IActionResult> GetSponsorshipBySponsorId([FromQuery] int id, string? searchTerm, string? sortColumn, string? sortOrder, int page, int pageSize)
        {
            var result = await _sponsorshipService.GetSponsorshipBySponsorId(id, searchTerm, sortColumn, sortOrder, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "3, 5")]
        public async Task<IActionResult> CreateSponsorship(CreateSponsorshipDTO createSponsorshipDTO)
        {
            var result = await _sponsorshipService.CreateSponsorship(createSponsorshipDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "3, 5")]
        public async Task<IActionResult> UpdateSponsorship(int id, [FromBody] CreateSponsorshipDTO createSponsorshipDTO)
        {
            var result = await _sponsorshipService.UpdateSponsorship(id, createSponsorshipDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "3, 5")]
        public async Task<IActionResult> DeleteSponsorship(int id)
        {
            var result = await _sponsorshipService.DeleteSponsorship(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
