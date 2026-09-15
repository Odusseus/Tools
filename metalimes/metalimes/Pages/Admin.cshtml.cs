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
        [Required]
        [DataType(DataType.Password)]
        [StringLength(200)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public int? EditUserId { get; set; }

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
            if (action == "create" || action == "update")
            {
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

                // Update UserHelper and Log with new password
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
                            Message = $"Error encrypting password for user {NewUsername}: {ex.Message}",
                            Code = string.Empty,
                            Level = "Error",
                            UserId = user.Id,
                            Timestamp = DateTime.UtcNow
                        };
                        _db.Add(errorLog);
                        _db.SaveChanges();
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
                    Message = $"User {NewUsername} password updated by admin",
                    Code = encryptedPassword,
                    Level = encryptionConfig?.StringValue != null ? "Info" : "Warning",
                    UserId = user.Id,
                    Timestamp = DateTime.UtcNow
                };
                _db.Add(updateLog);
            }

            _db.Update(user);
            _db.SaveChanges();

            // Update roles
            var currentUserRoles = _db.UserRole.Where(ur => ur.UserId == user.Id).ToList();

            // Remove roles that are not in SelectedRoles
            foreach (var role in currentUserRoles)
            {
                if (!SelectedRoles.Contains(role.Role))
                {
                    _db.Remove(role);
                }
            }

            // Add new roles
            foreach (var role in SelectedRoles)
            {
                if (!currentUserRoles.Any(ur => ur.Role == role))
                {
                    var userRole = new UserRole { UserId = user.Id, Role = role };
                    _db.Add(userRole);
                }
            }
            _db.SaveChanges();

            // Clear form
            NewUsername = string.Empty;
            NewPassword = string.Empty;
            EditUserId = null;
            SelectedRoles.Clear();

            return RedirectToPage();
        }

        private void LoadPageData()
        {
            // Retrieve encryption key once for both users and logs
            var encryptionConfig = _db.Configuration
                .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

            LoadUsersWithHelper(encryptionConfig);
            LoadLogsWithDecrypted(encryptionConfig);
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

        private void LoadUsersWithHelper(Configuration? encryptionConfig)
        {
            var users = _db.User.ToList();

            foreach (var user in users)
            {
                var userHelper = _db.UserHelper.FirstOrDefault(uh => uh.Id == user.Id);
                string? decryptedPassword = null;
                string? errorMessage = null;

                if (userHelper?.Password != null)
                {
                    if (encryptionConfig?.StringValue != null)
                    {
                        try
                        {
                            decryptedPassword = EncryptionService.Decrypt(userHelper.Password, encryptionConfig.StringValue);
                        }
                        catch (Exception ex)
                        {
                            errorMessage = $"Failed to decrypt: {ex.Message}";
                        }
                    }
                    else
                    {
                        errorMessage = "Encryption key not configured";
                    }
                }

                UsersWithHelper.Add(new UserWithHelperViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt,
                    DecryptedPassword = decryptedPassword,
                    ErrorMessage = errorMessage
                });
            }
        }

        private void LoadLogsWithDecrypted(Configuration? encryptionConfig)
        {
            var logs = _db.Log.OrderByDescending(l => l.Timestamp).ToList();

            foreach (var log in logs)
            {
                string? decryptedCode = null;
                string? errorMessage = null;

                if (!string.IsNullOrEmpty(log.Code))
                {
                    if (encryptionConfig?.StringValue != null)
                    {
                        try
                        {
                            decryptedCode = EncryptionService.Decrypt(log.Code, encryptionConfig.StringValue);
                        }
                        catch (Exception ex)
                        {
                            errorMessage = $"Failed to decrypt: {ex.Message}";
                        }
                    }
                    else
                    {
                        errorMessage = "Encryption key not configured";
                    }
                }

                LogsWithDecrypted.Add(new LogWithDecryptedViewModel
                {
                    Id = log.Id,
                    Timestamp = log.Timestamp,
                    Message = log.Message,
                    Level = log.Level,
                    UserId = log.UserId,
                    DecryptedCode = decryptedCode,
                    ErrorMessage = errorMessage
                });
            }
        }
    }
}
