using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SafeVault.Utilities
{
    public static class JwtHelper
    {
        // ✅ Secure and minimum-length key (≥ 32 chars)
        private const string SecretKey = "SafeVaultJwtKey_2025@SecureEncryption!";  // 🔒 Move to config/env later
        private const string Issuer = "SafeVaultIssuer";
        private const string Audience = "SafeVaultClient";

        public static string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);  // ✅ 32+ byte key
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}