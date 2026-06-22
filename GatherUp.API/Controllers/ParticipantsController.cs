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
    public class ParticipantsController : ControllerBase
    {
        private readonly ParticipantService _participants;
        private readonly EventService _events;

        public ParticipantsController(ParticipantService participants, EventService events)
        {
            _participants = participants;
            _events = events;
        }

        int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        bool IsManagerOf(int eventId)
        {
            var ev = _events.GetById(eventId);
            return ev.ManagerId == CallerId;
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetEventParticipants(int eventId) =>
            Ok(_participants.GetEventParticipants(eventId));

        [HttpGet("event/{eventId}/pending")]
        public IActionResult GetPending(int eventId) =>
            Ok(_participants.GetPendingParticipants(eventId));

        /// <summary>Add participant — manager of this event only.</summary>
        [HttpPost("event/{eventId}")]
        public IActionResult AddParticipant(int eventId, [FromBody] Participant participant)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _participants.AddParticipantToEvent(participant, eventId);
            return Ok(new { participant.Id, participant.Name });
        }

        /// <summary>Confirm attendance — the participant themselves.</summary>
        [HttpPut("{participantId}/attendance")]
        public IActionResult ConfirmAttendance(int participantId, [FromQuery] bool isAttending)
        {
            _participants.ConfirmAttendance(participantId, isAttending);
            return NoContent();
        }

        /// <summary>Confirm payment — manager of the event only.</summary>
        [HttpPut("{participantId}/payment")]
        public IActionResult ConfirmPayment(int participantId, [FromQuery] decimal amount, [FromQuery] int eventId)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _participants.ConfirmPayment(participantId, amount);
            return NoContent();
        }

        /// <summary>Send invitations — manager of this event only.</summary>
        [HttpPost("event/{eventId}/invite")]
        public IActionResult SendInvitations(int eventId, [FromQuery] string eventDescription, [FromQuery] string bankDetails)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _participants.SendInvitations(eventId, eventDescription, bankDetails);
            return Ok(new { message = "Invitations sent successfully." });
        }

        /// <summary>Send payment reminders — manager of this event only.</summary>
        [HttpPost("event/{eventId}/remind-payment")]
        public IActionResult SendPaymentReminders(int eventId, [FromQuery] string bankDetails)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _participants.SendPaymentReminders(eventId, bankDetails);
            return Ok(new { message = "Payment reminders sent successfully." });
        }

        /// <summary>Update notification preferences — the participant themselves.</summary>
        [HttpPut("{participantId}/preferences")]
        public IActionResult UpdatePreferences(int participantId, [FromQuery] ContactPreference preference)
        {
            _participants.UpdateContactPreference(participantId, preference);
            return NoContent();
        }
    }
}
