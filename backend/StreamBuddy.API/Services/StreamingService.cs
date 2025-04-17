namespace StreamBuddy.API.Services
{
    public class StreamingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiHost;
        private readonly string _baseUrl;

        // Constructor: read config (RapidAPI info, etc.)
        public StreamingService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            _apiKey = configuration["RapidAPI:ApiKey"]
                ?? throw new ArgumentNullException(nameof(_apiKey), "RapidAPI key is missing in appsettings.json");

            _apiHost = configuration["RapidAPI:ApiHost"]
                ?? throw new ArgumentNullException(nameof(_apiHost), "RapidAPI host is missing in appsettings.json");

            _baseUrl = configuration["RapidAPI:BaseUrl"]
                ?? throw new ArgumentNullException(nameof(_baseUrl), "Base URL is missing in appsettings.json");
        }

        // Example: get a show from RapidAPI by IMDB ID
        public async Task<string> GetShowAvailability(string type, string imdbId, string country = "us")
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(imdbId))
                throw new ArgumentException("Type and IMDB ID must be provided.");

            var validTypes = new HashSet<string> { "movie", "series" };
            if (!validTypes.Contains(type.ToLower()))
                throw new ArgumentException("Invalid type. Must be 'movie' or 'series'.");

            var endpoint = "/v2/get";
            var queryParams = new Dictionary<string, string>
            {
                { "country", country },
                { "imdb_id", imdbId }
            };

            return await SendRequestAsync(endpoint, queryParams);
        }

        // Example: Search movies by a keyword
        public async Task<string> SearchMovies(string query, string country = "us")
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                throw new ArgumentException("Search query must be at least 2 characters long.");

            var endpoint = "/search/ultra";
            var queryParams = new Dictionary<string, string>
            {
                { "query", query },
                { "country", country }
            };

            return await SendRequestAsync(endpoint, queryParams);
        }

        // INTERNAL: Actually send the HTTP request with the correct headers
        private async Task<string> SendRequestAsync(string endpoint, Dictionary<string, string> queryParams)
        {
            var fullUrl = $"{_baseUrl}{endpoint}";

            if (queryParams.Count > 0)
            {
                // Ensure "country=us" if not present
                if (!queryParams.ContainsKey("country"))
                {
                    queryParams["country"] = "us";
                }

                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                fullUrl += $"?{queryString}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("x-rapidapi-key", _apiKey);
            request.Headers.Add("x-rapidapi-host", _apiHost);

            Console.WriteLine($"📢 Sending Request: {fullUrl}");
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📢 API Response: {response.StatusCode} - {responseContent}");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Request Failed: {response.StatusCode} - {responseContent}");
            }

            return responseContent;
        }
    }
}