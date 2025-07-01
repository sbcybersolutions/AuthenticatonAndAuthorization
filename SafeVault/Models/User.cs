using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models
{
    public class User
    {
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid username.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}