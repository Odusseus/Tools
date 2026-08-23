using metalimes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace metalimes.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminModel : PageModel
    {
        private readonly AppDbContext _db;

        public AdminModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Users> Users { get; set; } = new();
        public List<Logs> Logs { get; set; } = new();

        public string AdminName { get; set; } = string.Empty;

        public void OnGet()
        {
            AdminName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            ViewData["Title"] = "Admin Dashboard";
            Users = _db.Users.ToList();
            Logs = _db.Logs.OrderByDescending(l => l.Timestamp).ToList();
        }
    }
}