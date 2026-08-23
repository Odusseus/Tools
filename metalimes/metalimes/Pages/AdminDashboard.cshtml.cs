using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using metalimes.Data;

namespace metalimes.Pages
{
    [Authorize]
    public class AdminDashboardModel : PageModel
    {
        private readonly AppDbContext _db;

        public AdminDashboardModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Users> Users { get; set; } = new();
        public List<Logs> Logs { get; set; } = new();

        public void OnGet()
        {
            ViewData["Title"] = "Admin Dashboard";
            Users = _db.Users.ToList();
            Logs = _db.Logs.OrderByDescending(l => l.Timestamp).ToList();
        }
    }
}