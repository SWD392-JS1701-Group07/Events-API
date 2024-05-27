using Events.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
