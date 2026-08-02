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
                Users newUser = new Users
                {
                    Username = Username,
                    Password = Password,
                    PasswordHash = new PasswordHasher<Users>().HashPassword(null, Password),
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties { IsPersistent = false });

                return RedirectToPage("/Bingo");
            }

            ModelState.AddModelError(string.Empty, "Ongeldige gebruikersnaam of wachtwoord.");
            return Page();
        }
    }
}