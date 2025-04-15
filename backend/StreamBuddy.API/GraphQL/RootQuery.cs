using System.Text.Json;
using StreamBuddy.API.Data;
using StreamBuddy.API.Models;
using StreamBuddy.API.Models.Shows;
using StreamBuddy.API.Services;


namespace StreamBuddy.API.GraphQL
{
    [GraphQLName("RootQuery")] 
    public class RootQuery
    {
        private readonly StreamingService _streamingService;

        public RootQuery(StreamingService streamingService)
        {
            _streamingService = streamingService;
        }

        // Example #1: Simple test
        public string Hello() => "Hello, StreamBuddy!";

        // Example #2: Return all movies from your local DB
        // [UseProjection] and [UseFiltering] are optional Hot Chocolate attributes if you want advanced features
        public IQueryable<Movie> GetAllMovies([Service] AppDbContext db) => db.Movies;

        // Example #3: Return all shows from your local DB
        public IQueryable<Show> GetAllShows([Service] AppDbContext db) => db.Shows;

        // Example #4: Use the external API to get a single show by IMDB ID, but do NOT store it in DB
        public async Task<Show> GetShowFromExternal(string imdbId, string country = "us")
        {
            var json = await _streamingService.GetShowAvailability("series", imdbId, country);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json) 
                       ?? new Dictionary<string, object>();

            // Build a Show object
            var show = new Show
            {
                ImdbId = imdbId,
                Title = data.ContainsKey("title") ? data["title"]?.ToString() : "Untitled",
                Poster = data.ContainsKey("poster") ? data["poster"]?.ToString() : null,
                Type = "series",
                // You can skip bridging or Availabilities for now
            };

            return show;
        }

        // Example #5: Searching for movies externally 
        public async Task<List<Movie>> SearchMoviesExternal(string query, string country = "us")
        {
            var json = await _streamingService.SearchMovies(query, country);
            var rootObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                         ?? new Dictionary<string, object>();

            // The result might be in rootObj["results"] as a List<Dictionary<string,object>>
            if (!rootObj.ContainsKey("results")) 
                return new List<Movie>();

            var results = rootObj["results"] as List<object> 
                          ?? new List<object>();

            var output = new List<Movie>();
            foreach (var item in results)
            {
                if (item is Dictionary<string, object> m)
                {
                    var movie = new Movie
                    {
                        ImdbId = m.GetValueOrDefault("imdb_id")?.ToString() ?? "",
                        Title = m.GetValueOrDefault("title")?.ToString() ?? "",
                        Overview = m.GetValueOrDefault("overview")?.ToString() ?? "",
                        // And so on
                    };
                    output.Add(movie);
                }
            }
            return output;
        }

        public async Task<Show> GetShowExternal(string imdbId, string country = "us")
        {
            var json = await _streamingService.GetShowAvailability("series", imdbId, country);

            // Parse the JSON into an internal object
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                    ?? new Dictionary<string, object>();

            return new Show
            {
                ImdbId = imdbId,
                Title = data.GetValueOrDefault("title")?.ToString() ?? "",
                Poster = data.GetValueOrDefault("poster")?.ToString() ?? "",
                // parse other fields from the external data
            };
        }
    }
}
