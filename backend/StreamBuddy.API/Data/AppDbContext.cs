using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<StreamingOption> StreamingOptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.StreamingPlatforms)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reactions)
                .HasForeignKey(r => r.UserId);
            
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Movie)
                .WithMany()
                .HasForeignKey(r => r.MovieId);
        }
    }
}
