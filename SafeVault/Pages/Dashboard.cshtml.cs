using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Pages
{
    public class DashboardModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        public void OnGet()
        {
            // Username is automatically bound from query string
        }
    }
}