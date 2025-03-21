using System.Text.Json;

namespace StreamBuddy.API.GraphQL;

public class Movie
{
    public string ImdbId { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Poster { get; set; }
    public Dictionary<string, string> StreamingOptions { get; set; }
}

public class Query
{
    private readonly StreamingService _streamingService;

    public Query(StreamingService streamingService)
    {
        _streamingService = streamingService;
    }

    public string Hello() => "Hello, StreamBuddy!";

    public async Task<Movie> GetShowAvailability(string type, string id, string country = "us")
    {
        var json = await _streamingService.GetShowAvailability(type, id, country);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return new Movie
        {
            ImdbId = id,
            Title = data["title"]?.ToString(),
            Type = type,
            Poster = data["poster"]?.ToString(),
            StreamingOptions = data["streamingInfo"] as Dictionary<string, string>
        };
    }

public async Task<List<Movie>> SearchMovies(string query, string country = "us")
    {
        try
        {
            var json = await _streamingService.SearchMovies(query, country);
            Console.WriteLine($"📢 SearchMovies JSON: {json}");
            var data = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(json);
            return data["results"].Select(item => new Movie
            {
                ImdbId = item["imdb_id"]?.ToString(),
                Title = item["title"]?.ToString(),
                Type = item["type"]?.ToString(),
                Poster = item["poster"]?.ToString(),
                StreamingOptions = item["streamingInfo"] as Dictionary<string, string>
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SearchMovies Error: {ex.Message}");
            throw; // Let GraphQL handle the error
        }
    }
}