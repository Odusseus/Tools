using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using metalimes.Data;

namespace metalimes.Pages
{
    public class AllEventsModel : PageModel
    {
        private readonly AppDbContext _db;

        public AllEventsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Event> Events { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? BeginDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? NameFilter { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "All Events";
            LoadEvents();
        }

        private void LoadEvents()
        {
            var query = _db.Event.AsNoTracking().AsQueryable();

            if (BeginDateFilter.HasValue)
            {
                var beginDate = BeginDateFilter.Value.Date;
                query = query.Where(e => e.BeginDate.Date >= beginDate);
            }

            if (EndDateFilter.HasValue)
            {
                var endDate = EndDateFilter.Value.Date;
                query = query.Where(e => e.EndDate.Date <= endDate);
            }

            if (!string.IsNullOrWhiteSpace(NameFilter))
            {
                var name = NameFilter.Trim();
                query = query.Where(e => EF.Functions.Like(e.Name, $"%{name}%"));
            }

            Events = query
                .OrderBy(e => e.BeginDate)
                .ThenBy(e => e.Name)
                .ToList();
        }
    }
}
