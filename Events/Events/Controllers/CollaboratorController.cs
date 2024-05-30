using Events.Business.Interfaces;
using Events.Data;
using Events.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;

        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService;
        }

        [HttpGet]
        //[Authorize(Roles = "Staff,Event operator")]
        public async Task<IActionResult> GetAllCollaborators()
        {
            var collaborators = await _collaboratorService.GetAllCollaborators();
            return Ok(collaborators);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Staff,Event operator")]
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
        //[Authorize(Roles = "Staff,Event operator")]
        public async Task<IActionResult> SearchCollaborators(int? accountId, int? eventId, Enums.CollaboratorStatus? collabStatus)
        {
            var collaborators = await _collaboratorService.SearchCollaborators(accountId, eventId, collabStatus);
            return Ok(collaborators);
        }
        [HttpPost("register")]
       //[Authorize(Roles = "Visitor")]
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