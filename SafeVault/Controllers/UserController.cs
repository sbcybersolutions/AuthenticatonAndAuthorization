using Microsoft.AspNetCore.Mvc;
using SafeVault.Services;
using SafeVault.Utilities;
using SafeVault.Models;

namespace SafeVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            string cleanUsername = InputSanitizer.Sanitize(username);

            if (!InputSanitizer.IsValidUsername(cleanUsername))
                return BadRequest("Invalid username format.");

            var user = await _userService.GetUserByUsernameAsync(cleanUsername);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            if (!InputSanitizer.IsValidUsername(request.Username))
                return BadRequest("Invalid username format.");

            var success = await _userService.RegisterAsync(
                request.Username.Trim(),
                request.Email.Trim(),
                request.Password
            );

            if (!success)
                return Conflict("Username is already taken.");

            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            return Ok("Login successful.");
        }

        [HttpGet("admin/dashboard")]
        public async Task<IActionResult> AdminDashboard([FromQuery] string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
                return Unauthorized("User not found.");

            if (user.Role != "Admin")
                return Forbid("Access denied. Admins only.");

            return Ok("Welcome to the Admin Dashboard.");
        }
    }
}