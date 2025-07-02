namespace SafeVault.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }    // 👈 Add this line if it's missing
        public string Password { get; set; }
    }
}