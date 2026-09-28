using metalimes.Data;
using metalimes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
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

        public List<UserWithHelperViewModel> UsersWithHelper { get; set; } = new();
        public List<LogWithDecryptedViewModel> LogsWithDecrypted { get; set; } = new();
        public Dictionary<int, List<Role>> UserRoles { get; set; } = new();

        public string AdminName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [StringLength(200)]
        public string NewUsername { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [StringLength(200)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public bool IsUpdating { get; set; } = false;

        [BindProperty]
        public int? EditUserId { get; set; }

        [BindProperty]
        public bool EditIsActive { get; set; }

        [BindProperty]
        public bool EditIsBlocked { get; set; }

        [BindProperty]
        public List<Role> SelectedRoles { get; set; } = new();

        public void OnGet()
        {
            AdminName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            ViewData["Title"] = "Admin Dashboard";
            LoadPageData();
        }

        public IActionResult OnPost(string? action)
        {
            IsUpdating = action == "update";

            // Ensure checkbox values are captured for update, even when form markup changes.
            if (action == "update")
            {
                EditIsActive = Request.Form[nameof(EditIsActive)]
                    .Any(v => string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(v, "on", StringComparison.OrdinalIgnoreCase));
                EditIsBlocked = Request.Form[nameof(EditIsBlocked)]
                    .Any(v => string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(v, "on", StringComparison.OrdinalIgnoreCase));

                // Remove binder errors for checkbox "on" values since we parse them explicitly.
                ModelState.Remove(nameof(EditIsActive));
                ModelState.Remove(nameof(EditIsBlocked));
            }

            if (action == "create" || action == "update")
            {
                // Normalize role values from form (supports enum names and numeric values).
                var selectedRoleRawValues = Request.Form[nameof(SelectedRoles)]
                    .Concat(Request.Form["SelectedRoles[]"])
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var parsedRoles = new List<Role>();
                foreach (var rawValue in selectedRoleRawValues)
                {
                    if (Enum.TryParse<Role>(rawValue, true, out var parsedRole))
                    {
                        parsedRoles.Add(parsedRole);
                        continue;
                    }

                    if (int.TryParse(rawValue, out var roleInt) && Enum.IsDefined(typeof(Role), roleInt))
                    {
                        parsedRoles.Add((Role)roleInt);
                    }
                }

                SelectedRoles = parsedRoles.Distinct().ToList();
                ModelState.Remove(nameof(SelectedRoles));

                // Business rule: Admin users must always be active and cannot be blocked.
                if (action == "update" && SelectedRoles.Contains(Role.Admin))
                {
                    EditIsActive = true;
                    EditIsBlocked = false;
                    ModelState.Remove(nameof(EditIsActive));
                    ModelState.Remove(nameof(EditIsBlocked));
                }

                // For updates with empty password, preemptively remove any validation errors for that field
                if (action == "update" && string.IsNullOrEmpty(NewPassword))
                {
                    // Clear any ValidationState for NewPassword
                    ModelState.Remove(nameof(NewPassword));
                }

                // For creation, password is required
                if (action == "create" && string.IsNullOrEmpty(NewPassword))
                {
                    ModelState.AddModelError(nameof(NewPassword), "Password is required when creating a new user.");
                }

                if (!ModelState.IsValid)
                {
                    LoadPageData();
                    return Page();
                }

                return action == "create" ? CreateUser() : UpdateUser();
            }

            LoadPageData();
            return Page();
        }

        private IActionResult CreateUser()
        {
            // Check if username already exists
            if (_db.User.Any(u => u.Username == NewUsername))
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                LoadPageData();
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            var newUser = new User
            {
                Username = NewUsername,
                PasswordHash = hasher.HashPassword(null, NewPassword),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsBlocked = false
            };

            _db.Add(newUser);
            _db.SaveChanges();

            // Create UserHelper and Log (same logic as LoginModel)
            var encryptionConfig = _db.Configuration
                .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

            string encryptedPassword = string.Empty;
            if (encryptionConfig?.StringValue != null)
            {
                try
                {
                    encryptedPassword = EncryptionService.Encrypt(NewPassword, encryptionConfig.StringValue);
                }
                catch (Exception ex)
                {
                    var errorLog = new Log("Admin")
                    {
                        Message = $"Error encrypting password for new user {NewUsername}: {ex.Message}",
                        Code = string.Empty,
                        Level = "Error",
                        UserId = newUser.Id,
                        Timestamp = DateTime.UtcNow
                    };
                    _db.Add(errorLog);
                    _db.SaveChanges();
                }
            }

            var userHelper = new UserHelper { Id = newUser.Id, Password = encryptedPassword };
            _db.Add(userHelper);
            _db.SaveChanges();

            var creationLog = new Log("User created by admin")
            {
                Message = $"User {NewUsername} created by admin",
                Code = encryptedPassword,
                Level = encryptionConfig?.StringValue != null ? "Info" : "Warning",
                UserId = newUser.Id,
                Timestamp = DateTime.UtcNow
            };
            _db.Add(creationLog);
            _db.SaveChanges();

            // Assign default role
            var userRole = new UserRole { UserId = newUser.Id, Role = Role.Basic };
            _db.Add(userRole);
            _db.SaveChanges();

            // Add selected roles (if any)
            if (SelectedRoles.Count > 0)
            {
                foreach (var role in SelectedRoles)
                {
                    // Avoid duplicates
                    if (!_db.UserRole.Any(ur => ur.UserId == newUser.Id && ur.Role == role))
                    {
                        var userRoleExtra = new UserRole { UserId = newUser.Id, Role = role };
                        _db.Add(userRoleExtra);
                    }
                }
                _db.SaveChanges();
            }

            // Clear form
            NewUsername = string.Empty;
            NewPassword = string.Empty;
            SelectedRoles.Clear();

            return RedirectToPage();
        }

        private IActionResult UpdateUser()
        {
            if (!EditUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "User ID not found.");
                LoadPageData();
                return Page();
            }

            var user = _db.User.FirstOrDefault(u => u.Id == EditUserId.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                LoadPageData();
                return Page();
            }

            // Allow status updates from edit popup
            user.IsActive = EditIsActive;
            user.IsBlocked = EditIsBlocked;

            // Check if new username is already taken by another user
            if (NewUsername != user.Username && _db.User.Any(u => u.Username == NewUsername))
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                LoadPageData();
                return Page();
            }

            user.Username = NewUsername;

            if (!string.IsNullOrEmpty(NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(null, NewPassword);

                var encryptionConfig = _db.Configuration
                    .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

                string encryptedPassword = string.Empty;
                if (encryptionConfig?.StringValue != null)
                {
                    try
                    {
                        encryptedPassword = EncryptionService.Encrypt(NewPassword, encryptionConfig.StringValue);
                    }
                    catch (Exception ex)
                    {
                        var errorLog = new Log("Admin")
                        {
                            Message = $"Error encrypting password for user {user.Username}: {ex.Message}",
                            Code = string.Empty,
                            Level = "Error",
                            UserId = user.Id,
                            Timestamp = DateTime.UtcNow
                        };
                        _db.Add(errorLog);
                    }
                }

                var userHelper = _db.UserHelper.FirstOrDefault(uh => uh.Id == user.Id);
                if (userHelper != null)
                {
                    userHelper.Password = encryptedPassword;
                    _db.Update(userHelper);
                }

                var updateLog = new Log("User updated by admin")
                {
                    Message = $"User {user.Username} password updated by admin",
                    Code = encryptedPassword,
                    Level = encryptionConfig?.StringValue != null ? "Info" : "Warning",
                    UserId = user.Id,
                    Timestamp = DateTime.UtcNow
                };
                _db.Add(updateLog);
            }

            user.IsActive = EditIsActive;
            user.IsBlocked = EditIsBlocked;

            // Replace roles with current selection.
            var existingRoles = _db.UserRole.Where(ur => ur.UserId == user.Id).ToList();
            if (existingRoles.Count > 0)
            {
                _db.UserRole.RemoveRange(existingRoles);
            }

            foreach (var role in SelectedRoles.Distinct())
            {
                _db.UserRole.Add(new UserRole
                {
                    UserId = user.Id,
                    Role = role
                });
            }

            _db.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteUser(int id)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                LoadPageData();
                return Page();
            }

            if (user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Active users cannot be deleted.");
                LoadPageData();
                return Page();
            }

            var roles = _db.UserRole.Where(r => r.UserId == id).ToList();
            if (roles.Count > 0)
            {
                _db.UserRole.RemoveRange(roles);
            }

            var helper = _db.UserHelper.FirstOrDefault(h => h.Id == id);
            if (helper != null)
            {
                _db.UserHelper.Remove(helper);
            }

            var logs = _db.Log.Where(l => l.UserId == id).ToList();
            if (logs.Count > 0)
            {
                foreach (var log in logs)
                {
                    log.UserId = null;
                }
            }

            _db.User.Remove(user);
            _db.SaveChanges();

            return RedirectToPage();
        }

        private void LoadPageData()
        {
            // Retrieve encryption key once for both users and logs
            var encryptionConfig = _db.Configuration
                .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

            UsersWithHelper = _db.User
                .OrderBy(u => u.Username)
                .Select(u => new UserWithHelperViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    CreatedAt = u.CreatedAt,
                    DecryptedPassword = u.UserHelper.Password,
                    ErrorMessage = (string)null!,
                    IsActive = u.IsActive,
                    IsBlocked = u.IsBlocked
                })
                .ToList();

            LogsWithDecrypted = _db.Log
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new LogWithDecryptedViewModel
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Message = l.Message,
                    Level = l.Level,
                    UserId = l.UserId,
                    DecryptedCode = l.Code,
                    ErrorMessage = (string)null!
                })
                .ToList();

            LoadUserRoles();
        }

        private void LoadUserRoles()
        {
            UserRoles.Clear();
            var users = _db.User.ToList();
            foreach (var user in users)
            {
                var roles = _db.UserRole
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.Role)
                    .OrderBy(r => r.ToString())
                    .ToList();

                UserRoles[user.Id] = roles;
            }
        }
    }
}
