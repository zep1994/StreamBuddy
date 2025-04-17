namespace StreamBuddy.API.DTO.Movies
{
    public class MovieDTO
    {
        public string ImdbId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ShowType { get; set; } = string.Empty;       
         public string Poster { get; set; } = string.Empty;
        public Dictionary<string, string>? StreamingPlatforms { get; internal set; }
    }
}    
        
        
    