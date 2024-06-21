using Events.Business.Services.Interfaces;
using Events.Models;
using Events.Models.DTOs.Request;
using Events.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
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
        public async Task<IActionResult> SearchCollaborators(int? accountId, int? eventId, string? collabStatus)
        {
            Enums.CollaboratorStatus? status = null;

            if (!string.IsNullOrEmpty(collabStatus))
            {
                if (!Enum.TryParse<Enums.CollaboratorStatus>(collabStatus, true, out var parsedStatus))
                {
                    return BadRequest("Invalid collaborator status value.");
                }
                status = parsedStatus;
            }

            var collaborators = await _collaboratorService.SearchCollaborators(accountId, eventId, status);
            return Ok(collaborators);
        }
        /// <summary>
        /// register
        /// </summary>
        /// <param name="createCollaboratorDto"></param>
        /// <returns></returns>
        [HttpPost]
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
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "5")]
        public async Task<IActionResult> ApproveCollaborator(int id)
        {
            var updatedCollaborator = await _collaboratorService.ApproveCollaboratorAsync(id);
            if (updatedCollaborator == null)
            {
                return NotFound();
            }

            return Ok(updatedCollaborator);
        }
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> CancelCollaborator(int id)
        {
            var updatedCollaborator = await _collaboratorService.CancelCollaboratorAsync(id);
            if (updatedCollaborator == null)
            {
                return NotFound();
            }

            return Ok(updatedCollaborator);
        }
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "5")]
        public async Task<IActionResult> RejectCollaborator(int id)
        {
            var updatedCollaborator = await _collaboratorService.RejectCollaboratorAsync(id);
            if (updatedCollaborator == null)
            {
                return NotFound();
            }

            return Ok(updatedCollaborator);
        }
    }
}