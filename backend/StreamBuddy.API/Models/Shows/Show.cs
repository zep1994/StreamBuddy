using StreamBuddy.API.Models.Platforms;
using StreamBuddy.API.Models.Users;

namespace StreamBuddy.API.Models.Shows
{
    public class Show
    {
        public int Id { get; set; }
        public string ImdbId { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public int? ReleaseYear { get; set; }
        public double? Rating { get; set; }

        public List<string> Genres { get; set; } = [];
        public List<string> Directors { get; set; } = [];
        public List<string> Cast { get; set; } = [];
        public List<string> Seasons { get; set; } = [];
        public List<string> Episodes { get; set; } = [];

        public string Title { get; set; } = string.Empty;
        public string? Type { get; set; } = "series";
        public string? Poster { get; set; }

        public List<Reaction> Reactions { get; set; } = [];

        public List<StreamingPlatform> StreamingPlatforms { get; set; } = [];
    }
}