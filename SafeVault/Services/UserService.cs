using Microsoft.EntityFrameworkCore;
using SafeVault.Database;
using SafeVault.Models;
using Isopoh.Cryptography.Argon2;

namespace SafeVault.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(string username, string email, string plainPassword)
        {
            // Check if username already exists
            bool exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists) return false;

            var passwordHash = Argon2.Hash(plainPassword);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginAsync(string username, string plainPassword)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || !Argon2.Verify(user.PasswordHash, plainPassword))
                return null;

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}