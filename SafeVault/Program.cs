using Microsoft.EntityFrameworkCore;
using SafeVault.Database;
using SafeVault.Services;



var builder = WebApplication.CreateBuilder(args);

// 📦 Register EF Core DbContext with connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SafeVaultMemoryDb"));

// 📌 Register custom services
builder.Services.AddScoped<UserService>();

// ✅ Enable API documentation via OpenAPI
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddRazorPages();

// ✅ Build the app
var app = builder.Build();



// 🔧 Enable OpenAPI only in Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

// ✅ Serve static files from wwwroot
app.UseStaticFiles();

app.UseHttpsRedirection();

// 🛣️ Add your endpoint mappings here (e.g., app.MapControllers())
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var adminUser = new SafeVault.Models.User
        {
            Username = "admin",
            Email = "admin@safevault.com",
            PasswordHash = Isopoh.Cryptography.Argon2.Argon2.Hash("AdminSecure123"),
            Role = "Admin"
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }

    app.MapRazorPages();
}

app.Run();