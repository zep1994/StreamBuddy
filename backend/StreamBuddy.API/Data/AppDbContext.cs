using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<StreamingOption> StreamingOptions { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.StreamingPlatforms)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);
        }
    }
}
