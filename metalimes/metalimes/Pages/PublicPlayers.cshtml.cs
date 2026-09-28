using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using metalimes.Data;
using System.ComponentModel.DataAnnotations;

namespace metalimes.Pages
{
    public class PublicPlayersModel : PageModel
    {
        private readonly AppDbContext _db;

        public PublicPlayersModel(AppDbContext db)
        {
            _db = db;
        }

        public Event? SelectedEvent { get; set; }
        public List<Player> Players { get; set; } = new();
        public List<PlayerPublic> PublicPlayers { get; set; } = new();

        [BindProperty]
        public CreatePlayerPublicInput NewPlayerPublic { get; set; } = new();

        public IActionResult OnGet(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            ViewData["Title"] = $"Players - {SelectedEvent.Name}";
            return Page();
        }

        public IActionResult OnPostCreate(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            var nonCreateKeys = ModelState.Keys
                .Where(k =>
                    !k.Equals("eventId", StringComparison.OrdinalIgnoreCase) &&
                    !k.StartsWith("NewPlayerPublic.", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var key in nonCreateKeys)
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Players - {SelectedEvent!.Name}";
                return Page();
            }

            var player = new PlayerPublic
            {
                FirstName = NewPlayerPublic.FirstName.Trim(),
                LastName = NewPlayerPublic.LastName.Trim(),
                Email = NewPlayerPublic.Email?.Trim() ?? string.Empty,
                FideId = NewPlayerPublic.FideId?.Trim() ?? string.Empty,
                Rating = NewPlayerPublic.Rating,
                Status = PlayerStatus.New,
                EventId = eventId,
                Timestamp = DateTime.UtcNow
            };

            _db.PlayerPublic.Add(player);
            _db.SaveChanges();

            return RedirectToPage(new { eventId });
        }

        private bool LoadEventAndPlayers(int eventId)
        {
            SelectedEvent = _db.Event.AsNoTracking().FirstOrDefault(e => e.Id == eventId);
            if (SelectedEvent == null)
            {
                return false;
            }

            Players = _db.Player
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();

            PublicPlayers = _db.PlayerPublic
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();

            return true;
        }

        public class CreatePlayerPublicInput
        {
            [Required]
            [StringLength(100)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [StringLength(100)]
            public string LastName { get; set; } = string.Empty;

            [EmailAddress]
            [StringLength(200)]
            public string? Email { get; set; }

            [StringLength(50)]
            public string? FideId { get; set; }

            [Range(0, 4000)]
            public int Rating { get; set; }
        }
    }
}
