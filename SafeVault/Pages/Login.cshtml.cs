// Pages/Login.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SafeVault.Pages
{
    public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        using var client = new HttpClient();

        var loginPayload = new { Username, Password };
        var content = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://localhost:5203/api/user/login", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Dashboard", new { username = Username });
        }

        Message = "Invalid username or password.";
        return Page();
    }
}
    
}

