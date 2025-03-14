using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Services
{
    public class StreamingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public StreamingService(IConfiguration configuration)
        {
           _apiKey = configuration["RAPIDAPI_KEY"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("❌ RapidAPI key is missing. Make sure it's in `.env` or `appsettings.json`.");
            }

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "streaming-availability.p.rapidapi.com");
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query)
        {
            var requestUrl = $"https://streaming-availability.p.rapidapi.com/shows/search/filters?series_granularity=show&order_direction=asc&order_by=original_title&genres_relation=and&output_language=en&show_type=movie&query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movies: {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return ParseApiResponse(jsonResponse);
        }

        public async Task<List<Movie>> GetMoviesByPlatformAsync(string platformName)
        {
            var requestUrl = $"https://streaming-availability.p.rapidapi.com/shows/search/filters?series_granularity=show&order_direction=asc&order_by=original_title&genres_relation=and&output_language=en&show_type=movie&streaming_platform={Uri.EscapeDataString(platformName)}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch movies: {response.ReasonPhrase}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return ParseApiResponse(jsonResponse);
        }

        private List<Movie> ParseApiResponse(string jsonResponse)
        {
            var movies = new List<Movie>();

            using var doc = JsonDocument.Parse(jsonResponse);
            var showsArray = doc.RootElement.GetProperty("shows");

            foreach (var show in showsArray.EnumerateArray())
            {
                var movie = new Movie
                {
                    Id = int.Parse(show.GetProperty("id").GetString() ?? "0"),
                    ImdbId = show.GetProperty("imdbId").GetString() ?? string.Empty,
                    TmdbId = show.GetProperty("tmdbId").GetString() ?? string.Empty,
                    Title = show.GetProperty("title").GetString() ?? string.Empty,
                    OriginalTitle = show.GetProperty("originalTitle").GetString() ?? string.Empty,
                    Overview = show.GetProperty("overview").GetString() ?? string.Empty,
                    ReleaseYear = show.GetProperty("releaseYear").GetInt32(),
                    Rating = show.GetProperty("rating").GetDouble(),
                    Genres = show.GetProperty("genres").EnumerateArray().Select(g => g.GetProperty("name").GetString() ?? "").ToList(),
                    Directors = show.GetProperty("directors").EnumerateArray().Select(d => d.GetString() ?? "").ToList(),
                    Cast = show.GetProperty("cast").EnumerateArray().Select(c => c.GetString() ?? "").ToList(),
                    StreamingPlatforms = show.GetProperty("streamingOptions").GetProperty("us")
                        .EnumerateArray()
                        .Select(s => new StreamingOption
                        {
                            ServiceId = s.GetProperty("service").GetProperty("id").GetString() ?? string.Empty,
                            ServiceName = s.GetProperty("service").GetProperty("name").GetString() ?? string.Empty,
                            SubscriptionType = s.GetProperty("type").GetString() ?? string.Empty,
                            Link = s.GetProperty("link").GetString() ?? string.Empty
                        })
                        .ToList()
                };

                movies.Add(movie);
            }

            return movies;
        }
    }
}
