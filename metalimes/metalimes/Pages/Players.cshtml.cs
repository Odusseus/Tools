using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PlayersModel(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public Event? SelectedEvent { get; set; }
        public List<Player> Players { get; set; } = new();
        public List<PlayerPublic> PublicPlayers { get; set; } = new();

        [BindProperty]
        public CreatePlayerInput NewPlayer { get; set; } = new();

        [BindProperty]
        public EditPlayerInput EditPlayer { get; set; } = new();

        [TempData]
        public string? FideErrorMessage { get; set; }

        public IActionResult OnGet(int eventId)
        {
            if (!LoadEventAndPlayers(eventId))
            {
                return NotFound();
            }

            ViewData["Title"] = $"Players - {SelectedEvent!.Name}";
            return Page();
        }

        public async Task<IActionResult> OnPostCreate(int eventId)
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

            var createLookup = await ApplyFideLookupAsync(player);
            if (!createLookup.Success && !string.IsNullOrWhiteSpace(createLookup.ErrorMessage))
            {
                FideErrorMessage = createLookup.ErrorMessage;
            }

            _db.Player.Add(player);
            await _db.SaveChangesAsync();

            return RedirectToPage(new { eventId });
        }

        public async Task<IActionResult> OnPostImportPublic(int eventId, int publicId)
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

            var importLookup = await ApplyFideLookupAsync(player);
            if (!importLookup.Success && !string.IsNullOrWhiteSpace(importLookup.ErrorMessage))
            {
                FideErrorMessage = importLookup.ErrorMessage;
            }

            _db.Player.Add(player);
            publicPlayer.Status = PlayerStatus.Imported;
            await _db.SaveChangesAsync();

            return RedirectToPage(new { eventId });
        }

        public async Task<IActionResult> OnPostEdit(int eventId)
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

            var editLookup = await ApplyFideLookupAsync(player);
            if (!editLookup.Success && !string.IsNullOrWhiteSpace(editLookup.ErrorMessage))
            {
                FideErrorMessage = editLookup.ErrorMessage;
            }

            await _db.SaveChangesAsync();

            return RedirectToPage(new { eventId });
        }

        public async Task<IActionResult> OnPostRecheckFide(int eventId, int id)
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

            var recheckLookup = await ApplyFideLookupAsync(player);
            if (!recheckLookup.Success && !string.IsNullOrWhiteSpace(recheckLookup.ErrorMessage))
            {
                FideErrorMessage = recheckLookup.ErrorMessage;
            }
            await _db.SaveChangesAsync();

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

        private async Task<FideLookupResult> ApplyFideLookupAsync(Player player)
        {
            player.IsFideChecked = false;

            if (string.IsNullOrWhiteSpace(player.FideId))
            {
                return FideLookupResult.SuccessResult();
            }

            try
            {
                var apiKey = await _db.Configuration
                    .AsNoTracking()
                    .Where(c => c.Key == ConfigKey.ParseBotApiKey)
                    .Select(c => c.StringValue)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return FideLookupResult.Failure("ParseBot API key is not configured.");
                }

                var client = _httpClientFactory.CreateClient("ParseBot");
                var searchPath = _configuration["ParseBot:SearchPlayersPath"]
                    ?? "/scraper/9565d770-db40-4e26-8563-4394e5d962cf/search_players?query=";
                var requestUrl = $"{searchPath}{Uri.EscapeDataString(player.FideId)}";

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.TryAddWithoutValidation("X-API-Key", apiKey);

                using var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                player.FideLookupResponseJson = responseBody;

                if (!response.IsSuccessStatusCode)
                {
                    return FideLookupResult.Failure($"ParseBot error: {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                using var document = JsonDocument.Parse(responseBody);

                if (TryExtractRating(document.RootElement, out var rating))
                {
                    player.Rating = rating;
                    player.IsFideChecked = true;
                    return FideLookupResult.SuccessResult();
                }

                return FideLookupResult.Failure("ParseBot response did not contain a valid rating.");
            }
            catch (Exception ex)
            {
                return FideLookupResult.Failure($"ParseBot request failed: {ex.Message}");
            }
        }

        private sealed class FideLookupResult
        {
            public bool Success { get; private set; }
            public string? ErrorMessage { get; private set; }

            public static FideLookupResult SuccessResult() => new() { Success = true };
            public static FideLookupResult Failure(string message) => new() { Success = false, ErrorMessage = message };
        }

        private static bool TryExtractRating(JsonElement element, out int rating)
        {
            rating = 0;

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    var name = property.Name.ToLowerInvariant();

                    if (name.Contains("rating"))
                    {
                        if (property.Value.ValueKind == JsonValueKind.Number &&
                            property.Value.TryGetInt32(out var parsedNumber) &&
                            parsedNumber >= 0 && parsedNumber <= 4000)
                        {
                            rating = parsedNumber;
                            return true;
                        }

                        if (property.Value.ValueKind == JsonValueKind.String &&
                            int.TryParse(property.Value.GetString(), out var parsedString) &&
                            parsedString >= 0 && parsedString <= 4000)
                        {
                            rating = parsedString;
                            return true;
                        }
                    }

                    if (TryExtractRating(property.Value, out rating))
                    {
                        return true;
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (TryExtractRating(item, out rating))
                    {
                        return true;
                    }
                }
            }

            return false;
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
