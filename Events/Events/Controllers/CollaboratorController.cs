using Events.Business.Interfaces;
using Events.Data;
using Events.Data.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/collaborators")]
    [ApiVersionNeutral]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;

        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService;
        }

        [HttpGet]
        [Authorize(Roles = "4, 5")]
        public async Task<IActionResult> GetAllCollaborators()
        {
            var collaborators = await _collaboratorService.GetAllCollaborators();
            return Ok(collaborators);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "4, 5")]
        public async Task<IActionResult> GetCollaboratorById(int id)
        {
            var collaborator = await _collaboratorService.GetCollaboratorById(id);
            if (collaborator == null)
            {
                return NotFound();
            }
            return Ok(collaborator);
        }

        [HttpGet("search")]
        [Authorize(Roles = "4, 5")]
        public async Task<IActionResult> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var collaborators = await _collaboratorService.SearchCollaborators(accountId, eventId, collabStatus);
            return Ok(collaborators);
        }
        [HttpPost("register")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> RegisterCollaborator([FromBody] CreateCollaboratorDTO createCollaboratorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collaborator = await _collaboratorService.CreateCollaborator(createCollaboratorDto);

            return CreatedAtAction(nameof(GetCollaboratorById), new { id = collaborator.Id }, collaborator);
        }
    }
}