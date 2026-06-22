using GatherUp.BL.Services;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly EventService _events;

        public EventsController(EventService events) => _events = events;

        int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id) => Ok(_events.GetById(id));

        /// <summary>Get all events for the logged-in user with their role per event.</summary>
        [HttpGet("mine")]
        public IActionResult GetMyEvents() => Ok(_events.GetAllEventsForUser(CallerId));

        [HttpGet("user/{userId}")]
        public IActionResult GetUserEvents(int userId) => Ok(_events.GetAllEventsForUser(userId));

        /// <summary>Create a new event — any authenticated user can create one (becomes the manager).</summary>
        [HttpPost]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            // Creator always becomes the manager
            newEvent.ManagerId = CallerId;
            _events.CreateEvent(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        /// <summary>Update event — only the manager of THIS event may do so.</summary>
        [HttpPut]
        public IActionResult UpdateEvent([FromBody] Event updatedEvent)
        {
            var ev = _events.GetById(updatedEvent.Id);
            if (ev.ManagerId != CallerId) return Forbid();
            _events.UpdateEventDetails(updatedEvent);
            return NoContent();
        }
    }
}
