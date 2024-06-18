using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Events.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Events.Utils;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using Events.Business.Services.Interfaces;
using static Events.Utils.Enums;

namespace Events.API.Controllers
{

    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/events")]
    [ApiVersionNeutral]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllEvents([FromQuery] string? searchTerm, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var events = await _eventService.GetAllEvents(searchTerm, sortColumn, sortOrder, page, pageSize);
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
     //   [Authorize(Roles = "5")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO createEventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set the event status
            createEventDTO.EventStatus = EventStatus.Planning.ToString();

            // Create the event
            var response = await _eventService.CreateEvent(createEventDTO);

            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            var createdEvent = response.Data as EventDTO;

            return CreatedAtAction(nameof(CreateEvent), new { id = createdEvent.Id }, createdEvent);
        }


        /// <summary>
        /// approve-events
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
       // [Authorize(Roles = "4")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventStatus(int id)
        {
            var eventToUpdate = await _eventService.GetEventById(id);
            if (eventToUpdate == null)
            {
                return NotFound();
            }

            // Fix the status to "Ongoing"
            var newStatus = EventStatus.Ongoing;
            await _eventService.UpdateStatus(id, newStatus);
            return Ok(eventToUpdate);
        }
        ///
        [HttpGet("needing-approval")]
      //  [Authorize(Roles = "4")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsNeedingApproval()
        {
            var eventStatus = EventStatus.Planning;
            var events = await _eventService.GetEventsByStatus(eventStatus);
            return Ok(events);
        }
        [HttpPut("{id}/update-details")]
      //  [Authorize(Roles = "5")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEventDetails(int id, [FromBody] CreateEventDTO updateEventDTO)
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

            try
            {
                await _eventService.UpdateEventDetails(id, updateEventDTO);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok(updateEventDTO);
        }
        /// <summary>
        /// delete-event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
         [Authorize(Roles = "5")]
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
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchEventsByName([FromQuery] string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return BadRequest("Event name cannot be empty.");
            }

            var events = await _eventService.SearchEventsByNameAsync(eventName);

            if (!events.Any())
            {
                return NotFound();
            }

            return Ok(events);
        }
        [HttpGet("{id}/name")]
        //[Authorize(Roles = "4,5")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventNameById(int id)
        {
            try
            {
                var eventName = await _eventService.GetEventNameByIdAsync(id);
                return Ok(new { EventName = eventName });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventExist = await _eventService.GetEventById(id);
                if (eventExist != null)
                {
                    return Ok(eventExist);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
