using System.ComponentModel.DataAnnotations;

namespace StreamBuddy.API.Models
{
    public class StreamingOption
    {
        [Key] // Primary Key
        public int Id { get; set; }

        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string SubscriptionType { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        // Foreign Key to Movie
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
