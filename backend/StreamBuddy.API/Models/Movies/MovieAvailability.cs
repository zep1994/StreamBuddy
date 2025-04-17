
using StreamBuddy.API.Models.Platforms;

namespace StreamBuddy.API.Models.Movies
{
    public class MovieAvailability
    {
        public int Id { get; set; }

        // Foreign keys to Movie and StreamingPlatform
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int PlatformId { get; set; }
        public StreamingPlatform? Platform { get; set; }

        // Time-limited availability
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
