using GatherUp.API.Services;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly JwtService _jwt;

        public AuthController(AuthService auth, JwtService jwt)
        {
            _auth = auth;
            _jwt = jwt;
        }

        public record LoginRequest(string Email, string PasswordId);
        public record RegisterRequest(string Name, string Email, string PasswordId);

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _auth.Login(req.Email, req.PasswordId);
            string role = user is EventManager ? "Manager" : user is EventHost ? "Host" : "Participant";
            var token = _jwt.GenerateToken(user, role);
            return Ok(new { token, role, userId = user.Id, name = user.Name });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            var user = _auth.Register(req.Name, req.Email, req.PasswordId);
            return Ok(new { userId = user.Id, name = user.Name });
        }
    }
}
