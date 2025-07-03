using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SafeVault.Services;
using SafeVault.Utilities;
using SafeVault.Models;
using Isopoh.Cryptography.Argon2;
using System.Security.Claims;

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

        // 🔍 Get a user by username
        [Authorize]
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

        // 📝 Register a new user
        [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterRequest request)
            {
                if (!InputSanitizer.IsValidUsername(request.Username))
                    return BadRequest("Invalid username.");

                if (!InputSanitizer.IsValidEmail(request.Email))
                    return BadRequest("Invalid email.");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Password required.");

                bool result = await _userService.RegisterAsync(
                    InputSanitizer.Sanitize(request.Username),
                    InputSanitizer.Sanitize(request.Email),
                    request.Password
                );

                if (!result)
                    return Conflict("Username already taken.");

                return Ok("Registration successful.");
            }

        // 🔐 Login and receive JWT token
        [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Username and password are required.");

                var user = await _userService.GetUserByUsernameAsync(request.Username);
                if (user == null || !Argon2.Verify(user.PasswordHash, request.Password))
                    return Unauthorized("Invalid credentials.");

                var token = JwtHelper.GenerateToken(user.Username, user.Role);
                return Ok(new { token });
            }

        // 🛡️ Secure Admin Dashboard with role-based authorization
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/dashboard")]
        public IActionResult AdminDashboard()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            return Ok($"Welcome to the Admin Dashboard, {username}.");
        }
    }
}