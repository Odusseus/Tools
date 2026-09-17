using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace metalimes.Pages
{
    [Authorize]
    public class MyWelcomeModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "My Welcome";
        }
    }
}
