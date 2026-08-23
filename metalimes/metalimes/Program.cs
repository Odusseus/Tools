using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using metalimes.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Authentication: cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

builder.Services.AddAuthorization();

// Register AppDbContext with SQLite provider using connection string from configuration.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5000); // http
    options.ListenAnyIP(7261, listenOptions => // https
    {
        listenOptions.UseHttps(); // will use dev-certs if available
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();   // maakt database + tabellen automatisch aan

    // Seed a default admin user if none exists (password = "password")
    //if (!db.Users.Any())
    //{
    //    var admin = new Users
    //    {
    //        Username = "admin",
    //        Role = "admin",
    //        CreatedAt = DateTime.UtcNow
    //    };
    //    var hasher = new PasswordHasher<Users>();
    //    admin.PasswordHash = hasher.HashPassword(admin, "password");
    //    db.Users.Add(admin);
    //    db.SaveChanges();
    //}
}

app.Run();