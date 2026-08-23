using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using metalimes.Data;

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

            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null)
            {
                string Role = "basic";
                if(Username == "metaadmin")
                {
                    Role = "admin";
                }

                Users newUser = new Users
                {
                    Username = Username,
                    Password = Password,
                    PasswordHash = new PasswordHasher<Users>().HashPassword(null, Password),
                    Role = Role,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Add(newUser);
                await _db.SaveChangesAsync();
                ModelState.AddModelError(string.Empty, "Ongeldige gebruikersnaam of wachtwoord.");
                return Page();
            }
            
            var hasher = new PasswordHasher<Users>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, Password);
            if (result == PasswordVerificationResult.Success)
            {
                Logs logs = new Logs("Login")
                {
                    Message = "Successful login attempt " + Username,
                    Level = "Info",
                    UserId = user.Id,
                    Timestamp = DateTime.UtcNow
                };

                // optionally persist the log:
                _db.Add(logs);
                await _db.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("role", user.Role)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                if (user.Username == "metaadmin")
                {
                    return RedirectToPage("/Admin");
                }
                return RedirectToPage("/Bingo");
            }
            else
            {
                Logs logs = new Logs("Login")
                {
                    Message = "Failed login attempt " + Username + " " +Password,
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