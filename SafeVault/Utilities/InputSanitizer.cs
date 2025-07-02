using System.Text.RegularExpressions;
using System.Net;

namespace SafeVault.Utilities
{
    public static class InputSanitizer
    {
        // Removes potentially dangerous characters and encodes HTML
        public static string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Encode HTML to prevent XSS
            string encoded = WebUtility.HtmlEncode(input);

            // Remove specific characters: single quote, double quote, semicolon, dash
            string cleaned = Regex.Replace(encoded, @"['"";\-]", string.Empty);

            // Optionally strip out any remaining HTML tags
            cleaned = Regex.Replace(cleaned, @"<.*?>", string.Empty);

            return cleaned.Trim();
        }

        // Validates email format
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Validates username (alphanumeric and underscores only)
        public static bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }
    }
}