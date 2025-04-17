using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Models;
using StreamBuddy.API.Models.Platforms;
using StreamBuddy.API.Models.Shows;
using StreamBuddy.API.Models.Users;

namespace StreamBuddy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        // Entities
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Show> Shows { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;

        // The bridging entity that also stores platform info
        public DbSet<StreamingPlatform> StreamingPlatforms { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ========== StreamingPlatform bridging ==========

            // If StreamingPlatform references a Movie
            modelBuilder.Entity<StreamingPlatform>()
                .HasOne(sp => sp.Movie)
                .WithMany(m => m.StreamingPlatforms)
                .HasForeignKey(sp => sp.MovieId)
                .OnDelete(DeleteBehavior.Restrict); 
                // or Cascade if you want to remove all sp when a Movie is removed

            // If StreamingPlatform references a Show
            modelBuilder.Entity<StreamingPlatform>()
                .HasOne(sp => sp.Show)
                .WithMany(s => s.StreamingPlatforms)
                .HasForeignKey(sp => sp.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== Reaction bridging ==========

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Show)
                .WithMany(s => s.Reactions)
                .HasForeignKey(r => r.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}