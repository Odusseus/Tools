using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public Event Event { get; set; } = new();

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

            Event.CreatedDate = DateTime.UtcNow;

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

            var eventToUpdate = _db.Event.FirstOrDefault(e => e.Id == EditingEventId.Value);
            if (eventToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "Event not found.");
                LoadUserEvents();
                return Page();
            }

            eventToUpdate.Name = Event.Name;
            eventToUpdate.BeginDate = Event.BeginDate;
            eventToUpdate.EndDate = Event.EndDate;

            _db.Event.Update(eventToUpdate);
            _db.SaveChanges();

            LoadUserEvents();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var eventToDelete = _db.Event.FirstOrDefault(e => e.Id == id);
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
            UserEvents = _db.Event
                .OrderByDescending(e => e.BeginDate)
                .ToList();
        }
    }
}
