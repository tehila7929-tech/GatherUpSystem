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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id) => Ok(_events.GetById(id));

        [HttpGet("user/{userId}")]
        public IActionResult GetUserEvents(int userId) => Ok(_events.GetAllEventsForUser(userId));

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            _events.CreateEvent(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        [HttpPut]
        public IActionResult UpdateEvent([FromBody] Event updatedEvent)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var ev = _events.GetById(updatedEvent.Id);
            if (ev.ManagerId != userId)
                return Forbid();
            _events.UpdateEventDetails(updatedEvent);
            return NoContent();
        }
    }
}
