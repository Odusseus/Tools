using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using metalimes.Data;

namespace metalimes.Pages
{
    [Authorize]
    public class EventsModel : PageModel
    {
        private readonly AppDbContext _db;

        public EventsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Event> UserEvents { get; set; } = new();

        [BindProperty]
        public Event Event { get; set; } = new()
        {
            BeginDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(1)
        };

        [BindProperty]
        public int? EditingEventId { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "Events";
            LoadUserEvents();
        }

        public IActionResult OnPostCreate()
        {
            if (!ModelState.IsValid)
            {
                LoadUserEvents();
                return Page();
            }

            var currentUserId = GetCurrentUserId();
            if (IsArbiter() && !currentUserId.HasValue)
            {
                return Forbid();
            }

            Event.CreatedDate = DateTime.UtcNow;
            Event.OwnerUserId = currentUserId;

            _db.Event.Add(Event);
            _db.SaveChanges();

            LoadUserEvents();
            return Page();
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid || !EditingEventId.HasValue)
            {
                LoadUserEvents();
                return Page();
            }

            var currentUserId = GetCurrentUserId();
            if (IsArbiter() && !currentUserId.HasValue)
            {
                return Forbid();
            }

            var eventToUpdate = _db.Event.FirstOrDefault(e =>
                e.Id == EditingEventId.Value &&
                (!IsArbiter() || e.OwnerUserId == currentUserId));

            if (eventToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "Event not found.");
                LoadUserEvents();
                return Page();
            }

            eventToUpdate.Name = Event.Name;
            eventToUpdate.BeginDate = Event.BeginDate;
            eventToUpdate.EndDate = Event.EndDate;
            eventToUpdate.Participants = Event.Participants;
            eventToUpdate.Rounds = Event.Rounds;

            _db.Event.Update(eventToUpdate);
            _db.SaveChanges();

            LoadUserEvents();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (IsArbiter() && !currentUserId.HasValue)
            {
                return Forbid();
            }

            var eventToDelete = _db.Event.FirstOrDefault(e =>
                e.Id == id &&
                (!IsArbiter() || e.OwnerUserId == currentUserId));

            if (eventToDelete == null)
            {
                return NotFound();
            }

            _db.Event.Remove(eventToDelete);
            _db.SaveChanges();

            LoadUserEvents();
            return Page();
        }

        private void LoadUserEvents()
        {
            var query = _db.Event.AsQueryable();
            var currentUserId = GetCurrentUserId();

            if (IsArbiter())
            {
                if (!currentUserId.HasValue)
                {
                    UserEvents = new List<Event>();
                    return;
                }

                query = query.Where(e => e.OwnerUserId == currentUserId.Value);
            }

            UserEvents = query
                .OrderBy(e => e.BeginDate)
                .ThenBy(e => e.Name)
                .ToList();
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var userId) ? userId : null;
        }

        private bool IsArbiter() => User.HasClaim("role", "arbiter");
    }
}
