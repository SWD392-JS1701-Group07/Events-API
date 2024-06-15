using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
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
        public async Task<IActionResult> GetAllSponsorship()
        {
            var result = await _sponsorshipService.GetAllSponsorship();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSponsorshipById(int id)
        {
            var result = await _sponsorshipService.GetSponsorshipById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSponsorship(CreateSponsorshipDTO createSponsorshipDTO)
        {
            var result = await _sponsorshipService.CreateSponsorship(createSponsorshipDTO);
            return StatusCode(result.StatusCode, result);
        }
    }
}
