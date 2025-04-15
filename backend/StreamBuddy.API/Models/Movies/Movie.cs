using StreamBuddy.API.Models.Platforms;
using StreamBuddy.API.Models.Users; 
using System.ComponentModel.DataAnnotations;

namespace StreamBuddy.API.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        public string ImdbId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public int? ReleaseYear { get; set; }
        public double? Rating { get; set; }

        public List<string> Genres { get; set; } = [];
        public List<string> Directors { get; set; } = [];
        public List<string> Cast { get; set; } = [];

        public List<Reaction> Reactions { get; set; } = [];
        public List<StreamingPlatform> StreamingPlatforms { get; set; } = [];
    }
}
