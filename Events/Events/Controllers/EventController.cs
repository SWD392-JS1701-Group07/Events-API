using Events.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Events.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            if (events == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(events);
            }
        }
        [HttpPost]
      //  [Authorize(Roles = "Event operator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO createEventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            createEventDTO.EventStatus = 0; // Set status to 0 when creating
            var createdEvent = await _eventService.CreateEvent(createEventDTO);
            return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
        }

        // Only EventApprover role can approve the event
        [HttpPut("{id}/approve")]
       // [Authorize(Roles = "Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var eventToApprove = await _eventService.GetEventById(id);
            if (eventToApprove == null)
            {
                return NotFound();
            }

            eventToApprove.EventStatus = 1; 
            await _eventService.UpdateEvent(eventToApprove);
            return Ok(eventToApprove);
        }

        [HttpGet("needing-approval")]
       // [Authorize(Roles = "Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsNeedingApproval()
        {
            var events = await _eventService.GetEventsNeedingApproval();
            return Ok(events);
        }
    }
}
