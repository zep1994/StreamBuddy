using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StreamBuddy.API.Models;
using System.Web;

namespace StreamBuddy.API.Services
{
    public class StreamingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public StreamingService(IConfiguration configuration)
        {
            _apiKey = configuration["RAPIDAPI_KEY"] ?? Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
            _baseUrl = $"https://{configuration["RAPIDAPI_HOST"] ?? "streaming-availability.p.rapidapi.com"}/shows";

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("❌ RapidAPI key is missing. Make sure it's in `.env` or `appsettings.json`.");
            }

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", configuration["RAPIDAPI_HOST"] ?? "streaming-availability.p.rapidapi.com");
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, string country = "us", string showType = "movie")
        {
            var queryParams = new Dictionary<string, string>
            {
                { "country", country },
                { "keyword", query },
                { "show_type", showType },
                { "output_language", "en" }
            };

            var requestUrl = $"{_baseUrl}/search/title?{BuildQueryString(queryParams)}";

            var response = await _httpClient.GetAsync(requestUrl);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Failed to fetch movies: {response.ReasonPhrase}\nAPI Response: {jsonResponse}");
                throw new Exception($"Failed to fetch movies: {response.ReasonPhrase}");
            }

            return ParseApiResponse(jsonResponse);
        }

        public async Task<List<Movie>> GetTopShowsAsync(string country, List<string> services)
        {
            var masterList = new List<Movie>();

            foreach (var service in services)
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "country", country },
                    { "service", service }
                };

                var requestUrl = $"{_baseUrl}/top?{BuildQueryString(queryParams)}";

                var response = await _httpClient.GetAsync(requestUrl);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Failed to fetch top shows from {service}: {response.ReasonPhrase}\nAPI Response: {jsonResponse}");
                    continue; 
                }

                var shows = ParseApiResponse(jsonResponse);
                masterList.AddRange(shows);
            }

            return masterList;
        }

        private async Task<List<Movie>> GetMoviesByPlatformAsync(string platform, string country = "us")
        {
            var queryParams = new Dictionary<string, string>
            {
                { "country", country },
                { "catalogs", platform },
                { "show_type", "movie" }
            };

            var requestUrl = $"{_baseUrl}/search/filters?{BuildQueryString(queryParams)}";

            var response = await _httpClient.GetAsync(requestUrl);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Failed to fetch platform-based movies: {response.ReasonPhrase}\nAPI Response: {jsonResponse}");
                throw new Exception($"Failed to fetch movies: {response.ReasonPhrase}");
            }

            return ParseApiResponse(jsonResponse);
        }

        private string BuildQueryString(Dictionary<string, string> parameters)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    query[param.Key] = param.Value;
                }
            }
            return query.ToString();
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
