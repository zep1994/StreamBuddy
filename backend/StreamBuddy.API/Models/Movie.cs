namespace StreamBuddy.API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ShowType { get; set; } = string.Empty;
        public string ImdbId { get; set; } = string.Empty;
        public string TmdbId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<string> Directors { get; set; } = new();
        public List<string> Cast { get; set; } = new();

        public List<StreamingOption> StreamingPlatforms { get; set; } = new();
    }
}
