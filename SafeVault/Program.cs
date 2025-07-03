using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SafeVault.Database;
using SafeVault.Models;
using SafeVault.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "SafeVaultIssuer",
        ValidAudience = "SafeVaultClient",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("SafeVaultJwtKey_2025@SecureEncryption!")) // 🔒 Secure this key!
    };
});

// 🗃️ Configure EF Core with in-memory DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SafeVaultMemoryDb"));

// 📌 Register custom services
builder.Services.AddScoped<UserService>();

// 🔍 Add controllers and Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// 📘 Optional: Add API documentation (OpenAPI/Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();

// 🛠️ Environment-specific configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

// 🚦 Enable middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();    // ✅ JWT auth middleware
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

// 🧪 Seed initial admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@safevault.com",
            PasswordHash = Isopoh.Cryptography.Argon2.Argon2.Hash("AdminSecure123"),
            Role = "Admin"
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.Run();