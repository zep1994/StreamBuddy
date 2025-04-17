using StreamBuddy.API.Models.Shows;

namespace StreamBuddy.API.Models.Platforms
{
    public class StreamingPlatform
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty; 
        public string SubscriptionType { get; set; } = string.Empty;
        public string? Link { get; set; } 

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int? ShowId { get; set; }
        public Show? Show { get; set; }
    }

}


