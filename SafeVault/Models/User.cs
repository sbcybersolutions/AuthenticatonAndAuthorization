using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary key

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid username.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Store Argon2 hashed password

        public string Role { get; set; } = UserRoles.Guest;  // Optional, for future role-based access
    }
}