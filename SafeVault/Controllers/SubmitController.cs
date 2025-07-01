using Microsoft.AspNetCore.Mvc;
using SafeVault.Models;
using SafeVault.Utilities;
using System.Data.SqlClient;

namespace SafeVault.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubmitController : ControllerBase
    {
        private readonly string _connectionString = "YourConnectionStringHere";

        [HttpPost("/submit")]
        public IActionResult Submit([FromForm] User user)
        {
            // Sanitize inputs
            string cleanUsername = InputSanitizer.Sanitize(user.Username);
            string cleanEmail = InputSanitizer.Sanitize(user.Email);

            // Validate inputs
            if (!InputSanitizer.IsValidUsername(cleanUsername) || !InputSanitizer.IsValidEmail(cleanEmail))
                return BadRequest("Invalid input format.");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Users (Username, Email) VALUES (@Username, @Email)", conn);
                cmd.Parameters.AddWithValue("@Username", cleanUsername);
                cmd.Parameters.AddWithValue("@Email", cleanEmail);
                cmd.ExecuteNonQuery();
            }

            return Ok("User submitted successfully");
        }
    }
}