using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
