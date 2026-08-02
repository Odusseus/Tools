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

        public DbSet<Users> Users { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User.Username moet uniek zijn
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Relatie Log → User (optioneel)
            modelBuilder.Entity<Logs>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
