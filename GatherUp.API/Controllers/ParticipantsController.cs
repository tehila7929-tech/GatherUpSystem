using GatherUp.BL.Services;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        private readonly ParticipantService _participants;

        public ParticipantsController(ParticipantService participants) => _participants = participants;

        [HttpGet("event/{eventId}")]
        public IActionResult GetEventParticipants(int eventId) =>
            Ok(_participants.GetEventParticipants(eventId));

        [HttpGet("event/{eventId}/pending")]
        public IActionResult GetPending(int eventId) =>
            Ok(_participants.GetPendingParticipants(eventId));

        [HttpPost("event/{eventId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult AddParticipant(int eventId, [FromBody] Participant participant)
        {
            _participants.AddParticipantToEvent(participant, eventId);
            return Ok();
        }

        [HttpPut("{participantId}/attendance")]
        public IActionResult ConfirmAttendance(int participantId, [FromQuery] bool isAttending)
        {
            _participants.ConfirmAttendance(participantId, isAttending);
            return NoContent();
        }

        [HttpPut("{participantId}/payment")]
        [Authorize(Roles = "Manager")]
        public IActionResult ConfirmPayment(int participantId, [FromQuery] decimal amount)
        {
            _participants.ConfirmPayment(participantId, amount);
            return NoContent();
        }
    }
}
