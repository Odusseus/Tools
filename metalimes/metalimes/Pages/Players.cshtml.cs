using System.ComponentModel.DataAnnotations;
using metalimes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace metalimes.Pages
{
    [Authorize]
    public class PlayersModel : PageModel
    {
        private readonly AppDbContext _db;

        public PlayersModel(AppDbContext db)
        {
            _db = db;
        }

        public Event? SelectedEvent { get; set; }
        public List<Player> Players { get; set; } = new();
        public List<PlayerPublic> PublicPlayers { get; set; } = new();

        [BindProperty]
        public CreatePlayerInput NewPlayer { get; set; } = new();

        [BindProperty]
        public EditPlayerInput EditPlayer { get; set; } = new();

        public IActionResult OnGet(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            ViewData["Title"] = $"Players - {SelectedEvent!.Name}";
            return Page();
        }

        public IActionResult OnPostCreate(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            // This post only validates NewPlayer fields.
            // Remove any model state entries from other form sections.
            var nonCreateKeys = ModelState.Keys
                .Where(k =>
                    !k.Equals("eventId", StringComparison.OrdinalIgnoreCase) &&
                    !k.StartsWith("NewPlayer.", StringComparison.OrdinalIgnoreCase))
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

            var player = new Player
            {
                FirstName = NewPlayer.FirstName.Trim(),
                LastName = NewPlayer.LastName.Trim(),
                Email = NewPlayer.Email?.Trim() ?? string.Empty,
                FideId = NewPlayer.FideId?.Trim() ?? string.Empty,
                Rating = NewPlayer.Rating,
                Status = NewPlayer.Status,
                EventId = eventId
            };

            _db.Player.Add(player);
            _db.SaveChanges();

            return RedirectToPage(new { eventId });
        }

        public IActionResult OnPostImportPublic(int eventId, int publicId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            var publicPlayer = _db.PlayerPublic.FirstOrDefault(p => p.Id == publicId && p.EventId == eventId);
            if (publicPlayer == null)
            {
                return NotFound();
            }

            if (publicPlayer.Status == PlayerStatus.Imported)
            {
                return RedirectToPage(new { eventId });
            }

            var player = new Player
            {
                FirstName = publicPlayer.FirstName,
                LastName = publicPlayer.LastName,
                Email = publicPlayer.Email,
                FideId = publicPlayer.FideId,
                Rating = publicPlayer.Rating,
                Status = PlayerStatus.New,
                EventId = eventId
            };

            _db.Player.Add(player);
            publicPlayer.Status = PlayerStatus.Imported;
            _db.SaveChanges();

            return RedirectToPage(new { eventId });
        }

        public IActionResult OnPostEdit(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            // This post only validates EditPlayer fields.
            // Remove any model state entries from other form sections.
            var nonEditKeys = ModelState.Keys
                .Where(k =>
                    !k.Equals("eventId", StringComparison.OrdinalIgnoreCase) &&
                    !k.StartsWith("EditPlayer.", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var key in nonEditKeys)
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Players - {SelectedEvent!.Name}";
                return Page();
            }

            var player = _db.Player.FirstOrDefault(p => p.Id == EditPlayer.Id && p.EventId == eventId);
            if (player == null)
            {
                ModelState.AddModelError(string.Empty, "Player not found.");
                ViewData["Title"] = $"Players - {SelectedEvent!.Name}";
                return Page();
            }

            player.FirstName = EditPlayer.FirstName.Trim();
            player.LastName = EditPlayer.LastName.Trim();
            player.Email = EditPlayer.Email?.Trim() ?? string.Empty;
            player.FideId = EditPlayer.FideId?.Trim() ?? string.Empty;
            player.Rating = EditPlayer.Rating;
            player.Status = EditPlayer.Status;

            _db.SaveChanges();

            return RedirectToPage(new { eventId });
        }

        public IActionResult OnPostDelete(int eventId, int id)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            var player = _db.Player.FirstOrDefault(p => p.Id == id && p.EventId == eventId);
            if (player == null)
            {
                return NotFound();
            }

            _db.Player.Remove(player);
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
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToList();

            PublicPlayers = _db.PlayerPublic
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToList();

            return true;
        }

        public class CreatePlayerInput
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

            public PlayerStatus Status { get; set; } = PlayerStatus.New;
        }

        public class EditPlayerInput
        {
            [Required]
            public int Id { get; set; }

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

            public PlayerStatus Status { get; set; } = PlayerStatus.New;
        }
    }
}
