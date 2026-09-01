using metalimes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace metalimes.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class ConfigurationManagementModel : PageModel
    {
        private readonly AppDbContext _db;

        public ConfigurationManagementModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Configuration> Configurations { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty]
        [Required]
        public ConfigKey Key { get; set; }

        [BindProperty]
        [Required]
        public string ValueType { get; set; } = "String"; // String, Integer, DateTime

        [BindProperty]
        public string? StringValue { get; set; }

        [BindProperty]
        public int? IntegerValue { get; set; }

        [BindProperty]
        public DateTime? DateTimeValue { get; set; }

        public Configuration? SelectedConfiguration { get; set; }

        public void OnGet(int? id)
        {
            ViewData["Title"] = "Configuration Management";
            LoadConfigurations();

            if (id.HasValue)
            {
                SelectedConfiguration = _db.Configuration.FirstOrDefault(c => c.Id == id.Value);
                if (SelectedConfiguration != null)
                {
                    EditId = SelectedConfiguration.Id;
                    Key = SelectedConfiguration.Key;
                    ValueType = SelectedConfiguration.ValueType;
                    StringValue = SelectedConfiguration.StringValue;
                    IntegerValue = SelectedConfiguration.IntegerValue;
                    DateTimeValue = SelectedConfiguration.DateTimeValue;
                }
            }
        }

        public IActionResult OnPost(string? action)
        {
            if (!ModelState.IsValid)
            {
                LoadConfigurations();
                return Page();
            }

            // Clear unused values based on ValueType
            if (ValueType == "String")
            {
                IntegerValue = null;
                DateTimeValue = null;
            }
            else if (ValueType == "Integer")
            {
                StringValue = null;
                DateTimeValue = null;
            }
            else if (ValueType == "DateTime")
            {
                StringValue = null;
                IntegerValue = null;
            }

            if (action == "create")
            {
                // Create new configuration
                if (_db.Configuration.Any(c => c.Key == Key))
                {
                    ModelState.AddModelError(string.Empty, "A configuration with this key already exists.");
                    LoadConfigurations();
                    return Page();
                }

                var newConfig = new Configuration
                {
                    Key = Key,
                    ValueType = ValueType,
                    StringValue = StringValue,
                    IntegerValue = IntegerValue,
                    DateTimeValue = DateTimeValue,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Configuration.Add(newConfig);
                _db.SaveChanges();

                return RedirectToPage();
            }
            else if (action == "update" && EditId.HasValue)
            {
                // Update existing configuration
                var config = _db.Configuration.FirstOrDefault(c => c.Id == EditId.Value);
                if (config == null)
                {
                    ModelState.AddModelError(string.Empty, "Configuration not found.");
                    LoadConfigurations();
                    return Page();
                }

                config.Key = Key;
                config.ValueType = ValueType;
                config.StringValue = StringValue;
                config.IntegerValue = IntegerValue;
                config.DateTimeValue = DateTimeValue;

                _db.Configuration.Update(config);
                _db.SaveChanges();

                return RedirectToPage();
            }
            else if (action == "delete" && EditId.HasValue)
            {
                // Delete configuration
                var config = _db.Configuration.FirstOrDefault(c => c.Id == EditId.Value);
                if (config != null)
                {
                    _db.Configuration.Remove(config);
                    _db.SaveChanges();
                }

                return RedirectToPage();
            }

            LoadConfigurations();
            return Page();
        }

        private void LoadConfigurations()
        {
            Configurations = _db.Configuration.OrderBy(c => c.Key).ToList();
        }
    }
}
