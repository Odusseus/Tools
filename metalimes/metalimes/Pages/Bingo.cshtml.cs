using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace metalimes.Pages
{
    [Authorize]
    public class BingoModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "Bingo";
        }
    }
}