using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
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
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<EventManager> _managers;
        private readonly IRepository<EventHost> _hosts;

        public EventsController(EventService events, IRepository<Participant> participants, IRepository<EventManager> managers, IRepository<EventHost> hosts)
        {
            _events = events;
            _participants = participants;
            _managers = managers;
            _hosts = hosts;
        }

        int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string CallerEmail => User.FindFirstValue(ClaimTypes.Email) ?? "";

        // Collect ALL IDs this person has across all role tables (by email)
        HashSet<int> AllCallerIds()
        {
            var email = CallerEmail;
            var ids = new HashSet<int> { CallerId };
            foreach (var p in _participants.GetAll().Where(p => p.Email == email)) ids.Add(p.Id);
            foreach (var m in _managers.GetAll().Where(m => m.Email == email))     ids.Add(m.Id);
            foreach (var h in _hosts.GetAll().Where(h => h.Email == email))        ids.Add(h.Id);
            return ids;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id) => Ok(_events.GetById(id));

        [HttpGet("mine")]
        public IActionResult GetMyEvents() => Ok(_events.GetAllEventsForUser(AllCallerIds()));

        [HttpGet("user/{userId}")]
        public IActionResult GetUserEvents(int userId) => Ok(_events.GetAllEventsForUser(new HashSet<int> { userId }));

        [HttpPost]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            newEvent.ManagerId = CallerId;
            _events.CreateEvent(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

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
