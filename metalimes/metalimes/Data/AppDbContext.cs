using metalimes.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace metalimes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserHelper> UserHelper { get; set; }
        public DbSet<Configuration> Configuration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User.Username moet uniek zijn
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Relatie Log → User (optioneel)
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // UserRole: mapping table, composite key (UserId, Role)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.Role });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ↔ UserHelper : one-to-one, shared primary key (User.Id == UserHelper.Id)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserHelper)
                .WithOne(uh => uh.User)
                .HasForeignKey<UserHelper>(uh => uh.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration table: Key is enum (unique)
            modelBuilder.Entity<Configuration>()
                .HasIndex(c => c.Key)
                .IsUnique();
        }
    }
}
