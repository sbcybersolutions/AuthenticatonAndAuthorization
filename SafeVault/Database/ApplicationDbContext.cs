using Microsoft.EntityFrameworkCore;
using SafeVault.Models;

namespace SafeVault.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}