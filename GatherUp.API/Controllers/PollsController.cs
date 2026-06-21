using GatherUp.BL.Services;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PollsController : ControllerBase
    {
        private readonly PollService _polls;

        public PollsController(PollService polls) => _polls = polls;

        [HttpGet("{pollId}/results")]
        public IActionResult GetResults(int pollId)
        {
            var (poll, results) = _polls.GetPollResults(pollId);
            return Ok(new { poll, results });
        }

        [HttpPost("event/{eventId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult CreatePoll(int eventId, [FromBody] Poll poll)
        {
            _polls.CreatePoll(poll, eventId);
            return Ok();
        }

        [HttpPost("{pollId}/question")]
        [Authorize(Roles = "Manager")]
        public IActionResult AddQuestion(int pollId, [FromBody] PollQuestion question)
        {
            _polls.AddQuestion(pollId, question);
            return Ok();
        }

        [HttpPost("{pollId}/answer")]
        public IActionResult SubmitAnswer(int pollId, [FromQuery] int questionId, [FromQuery] int participantId, [FromQuery] string answer)
        {
            _polls.SubmitAnswer(pollId, questionId, participantId, answer);
            return NoContent();
        }
    }
}
