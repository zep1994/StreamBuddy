
using StreamBuddy.API.Models.Shows;
using System.ComponentModel.DataAnnotations;

namespace StreamBuddy.API.Models.Users
{
    public class ViewingHistory
    {
        [Key]
        public int Id { get; set; }

        // Who watched it
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int? ShowId { get; set; }
        public Show? Show { get; set; }

        public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
        public int? DurationSeconds { get; set; }
    }
}
