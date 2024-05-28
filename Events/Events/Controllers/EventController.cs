using Events.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Events.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using static Events.Data.Enums;

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
        [HttpPost("created-events")]
      //  [Authorize(Roles = "Event operator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO createEventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            createEventDTO.EventStatus = (int)EventStatus.Planning; 
            var createdEvent = await _eventService.CreateEvent(createEventDTO);
            return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
        }

        // Only EventApprover role can approve the event
        [HttpPut("{id}/approve-events")]
        //[Authorize(Roles = "Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventStatus(int id, [FromQuery] EventStatus newStatus)
        {
            var eventToUpdate = await _eventService.GetEventById(id);
            if (eventToUpdate == null)
            {
                return NotFound();
            }

            await _eventService.UpdateStatus(id, newStatus);
            return Ok(eventToUpdate);
        }
        [HttpGet("needing-approval")]
        //[Authorize(Roles = "Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsNeedingApproval([FromQuery] EventStatus? status)
        {
            var eventStatus = status ?? EventStatus.Ongoing; 
            var events = await _eventService.GetEventsByStatus(eventStatus);
            return Ok(events);
        }
        [HttpPut("{id}/update-details")]
        //[Authorize(Roles = "Event operator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEventDetails(int id, [FromBody] EventDTO updateEventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventToUpdate = await _eventService.GetEventById(id);
            if (eventToUpdate == null)
            {
                return NotFound();
            }

            updateEventDTO.Id = id;
            await _eventService.UpdateEventDetails(updateEventDTO);

            return Ok(updateEventDTO);
        }
        [HttpDelete("{id}/delete-event")]
        // [Authorize(Roles = "Event operator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventToDelete = await _eventService.GetEventById(id);
            if (eventToDelete == null)
            {
                return NotFound();
            }

            await _eventService.DeleteEvent(id);
            return Ok();
        }





    }
}
