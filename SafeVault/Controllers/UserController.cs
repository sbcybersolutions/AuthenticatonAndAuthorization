using Microsoft.AspNetCore.Mvc;
using SafeVault.Services;
using SafeVault.Utilities;

namespace SafeVault.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController()
        {
            // Ideally use dependency injection; simplified here
            _userService = new UserService("YourConnectionStringHere");
        }

        [HttpGet("{username}")]
        public IActionResult GetUser(string username)
        {
            string cleanUsername = InputSanitizer.Sanitize(username);

            if (!InputSanitizer.IsValidUsername(cleanUsername))
                return BadRequest("Invalid username format.");

            var user = _userService.GetUserByUsername(cleanUsername);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }
    }
}