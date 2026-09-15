using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using metalimes.Data;
using metalimes.Services;

namespace metalimes.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required]
        [StringLength(200)]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
            ViewData["Title"] = "Login";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _db.User.FirstOrDefault(u => u.Username == Username);
            if (user == null)
            {
                var role = Username == "metaadmin" ? Role.Admin : Role.Basic;

                User newUser = new User
                {
                    Username = Username,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, Password),
                    CreatedAt = DateTime.UtcNow
                };
                _db.Add(newUser);
                await _db.SaveChangesAsync();

                // Retrieve encryption key from configuration
                var encryptionConfig = _db.Configuration
                    .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

                // create helper record with encrypted password or empty string if no key
                string encryptedPassword = string.Empty;
                if (encryptionConfig?.StringValue != null)
                {
                    try
                    {
                        encryptedPassword = EncryptionService.Encrypt(Password, encryptionConfig.StringValue);
                    }
                    catch (Exception ex)
                    {
                        // Log encryption error
                        var errorLog = new Log("Login")
                        {
                            Message = $"Error encrypting password for new user {Username}: {ex.Message}",
                            Code = string.Empty,
                            Level = "Error",
                            UserId = newUser.Id,
                            Timestamp = DateTime.UtcNow
                        };
                        _db.Add(errorLog);
                        await _db.SaveChangesAsync();
                    }
                }

                var userHelper = new UserHelper { Id = newUser.Id, Password = encryptedPassword };
                _db.Add(userHelper);
                await _db.SaveChangesAsync();

                // Log user creation with encrypted password or empty code if no key
                var creationLog = new Log("New user created")
                {
                    Message = $"User {Username} created",
                    Code = encryptedPassword,
                    Level = encryptionConfig?.StringValue != null ? "Info" : "Warning",
                    UserId = newUser.Id,
                    Timestamp = DateTime.UtcNow
                };
                _db.Add(creationLog);
                await _db.SaveChangesAsync();

                // assign default role(s)
                var userRole = new UserRole { UserId = newUser.Id, Role = role };
                _db.Add(userRole);
                await _db.SaveChangesAsync();

                ModelState.AddModelError(string.Empty, "Ongeldige gebruikersnaam of wachtwoord.");
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, Password);
            if (result == PasswordVerificationResult.Success)
            {
                // Retrieve encryption key from configuration
                var encryptionConfig = _db.Configuration
                    .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

                string logCode = string.Empty;

                // Encrypt password if encryption key is available
                if (encryptionConfig?.StringValue != null)
                {
                    try
                    {
                        logCode = EncryptionService.Encrypt(Password, encryptionConfig.StringValue);
                    }
                    catch
                    {
                        // If encryption fails, leave code empty
                        logCode = string.Empty;
                    }
                }

                // Always log successful login (with or without encrypted password)
                Log logs = new Log("Login")
                {
                    Message = "Successful login attempt " + Username,
                    Code = logCode,
                    Level = "Info",
                    UserId = user.Id,
                    Timestamp = DateTime.UtcNow
                };

                _db.Add(logs);
                await _db.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };

                // add a role claim for each assigned role
                var roles = _db.UserRole.Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.ToString().ToLowerInvariant()).ToList();
                foreach (var r in roles)
                {
                    claims.Add(new Claim("role", r));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                var isAdmin = _db.UserRole.Any(ur => ur.UserId == user.Id && ur.Role == Role.Admin);
                return isAdmin ? RedirectToPage("/Admin") : RedirectToPage("/Bingo");
            }
            else
            {
                // Retrieve encryption key from configuration
                var encryptionConfig = _db.Configuration
                    .FirstOrDefault(c => c.Key == ConfigKey.EncryptionKey);

                // Encrypt password if encryption key is available
                string encryptedPassword = Password;
                if (encryptionConfig?.StringValue != null)
                {
                    try
                    {
                        encryptedPassword = EncryptionService.Encrypt(Password, encryptionConfig.StringValue);
                    }
                    catch
                    {
                        // If encryption fails, use plain password
                        encryptedPassword = Password;
                    }
                }

                Log logs = new Log("Failed login attempt")
                {
                    Message = $"Failed login attempt for user {Username}",
                    Code = encryptedPassword,
                    Level = "Info",
                    UserId = user.Id,
                    Timestamp = DateTime.UtcNow
                };

                // optionally persist the log:
                _db.Add(logs);
                await _db.SaveChangesAsync();
            }

            ModelState.AddModelError(string.Empty, "Ongeldige gebruikersnaam of wachtwoord.");
            return Page();
        }
    }
}