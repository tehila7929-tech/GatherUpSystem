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
    public class PollsController : ControllerBase
    {
        private readonly PollService _polls;
        private readonly EventService _events;

        public PollsController(PollService polls, EventService events)
        {
            _polls = polls;
            _events = events;
        }

        int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        bool IsManagerOf(int eventId) => _events.GetById(eventId).ManagerId == CallerId;

        [HttpGet("{pollId}/results")]
        public IActionResult GetResults(int pollId)
        {
            var (poll, results) = _polls.GetPollResults(pollId);
            return Ok(new { poll, results });
        }

        /// <summary>Create poll — manager of this event only.</summary>
        [HttpPost("event/{eventId}")]
        public IActionResult CreatePoll(int eventId, [FromBody] Poll poll)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _polls.CreatePoll(poll, eventId);
            return Ok(new { poll.Id, poll.Name });
        }

        /// <summary>Add question — manager of this event only.</summary>
        [HttpPost("{pollId}/question")]
        public IActionResult AddQuestion(int pollId, [FromQuery] int eventId, [FromBody] PollQuestion question)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _polls.AddQuestion(pollId, question);
            return Ok();
        }

        /// <summary>Submit or change an answer — any authenticated user.</summary>
        [HttpPost("{pollId}/answer")]
        public IActionResult SubmitAnswer(int pollId, [FromQuery] int questionId, [FromQuery] int participantId, [FromQuery] string answer)
        {
            _polls.SubmitAnswer(pollId, questionId, participantId, answer);
            return NoContent();
        }
    }
}
