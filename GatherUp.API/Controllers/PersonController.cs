using GatherUp.BL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly AuthService _auth;

        public PersonController(AuthService auth) => _auth = auth;

        /// <summary>
        /// Get a user's profile by ID. Open to any authenticated user.
        /// </summary>
        [HttpGet("{userId}")]
        public IActionResult GetUser(int userId) => Ok(_auth.GetUserById(userId));

        /// <summary>
        /// Update name and email. Only the owner of the account may update it.
        /// </summary>
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserRequest req)
        {
            var callerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (callerId != userId)
                return Forbid();

            _auth.UpdateUser(userId, req.Name, req.Email);
            return NoContent();
        }

        public record UpdateUserRequest(string Name, string Email);
    }
}
