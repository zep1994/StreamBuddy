using StreamBuddy.API.Data;
using StreamBuddy.API.Models;
using StreamBuddy.API.Models.Shows;
using StreamBuddy.API.Models.Platforms;
using StreamBuddy.API.Services;
using System.Text.Json;

namespace StreamBuddy.API.GraphQL
{
    public class Mutation
    {
        private readonly StreamingService _streamingService;

        public Mutation(StreamingService streamingService)
        {
            _streamingService = streamingService;
        }

        // Example #1: Add a new Movie to the DB
        public async Task<Movie> AddMovie(Movie input, [Service] AppDbContext db)
        {
            db.Movies.Add(input);
            await db.SaveChangesAsync();
            return input;
        }

        // Example #2: Retrieve a show from the external API, then store it in the DB with bridging
        public async Task<Show> UpsertShowFromExternal(string imdbId, [Service] AppDbContext db, string country = "us")
        {
            var json = await _streamingService.GetShowAvailability("series", imdbId, country);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                       ?? new Dictionary<string, object>();

            // 1) Check if we already have a Show with that IMDB ID in DB
            var existingShow = db.Shows.FirstOrDefault(s => s.ImdbId == imdbId);

            // 2) If none found, create a new one
            if (existingShow == null)
            {
                existingShow = new Show
                {
                    ImdbId = imdbId,
                    Title = data.GetValueOrDefault("title")?.ToString() ?? "Untitled",
                    Overview = data.GetValueOrDefault("overview")?.ToString() ?? "",
                    ReleaseYear = data.ContainsKey("year") ? int.Parse(data["year"]?.ToString() ?? "0") : null,
                    Rating = null, // parse if available
                };
                db.Shows.Add(existingShow);
            }
            else
            {
                // (optional) update existing fields
                existingShow.Title = data.GetValueOrDefault("title")?.ToString() ?? existingShow.Title;
                existingShow.Overview = data.GetValueOrDefault("overview")?.ToString() ?? existingShow.Overview;
                // etc.
            }

            // 3) (Optional) parse "streamingInfo" 
            // Then create bridging platform records if you want
            if (data.ContainsKey("streamingInfo"))
            {
                var streamingInfo = data["streamingInfo"] as Dictionary<string, object> 
                                   ?? new Dictionary<string, object>();

                // e.g. "us" -> { "netflix": { ... }, "prime": { ... } }
                foreach (var regionKvp in streamingInfo)
                {
                    var region = regionKvp.Key; // "us"
                    if (regionKvp.Value is Dictionary<string, object> platforms)
                    {
                        // platforms: "netflix": { link: "...", ... }
                        foreach (var pfKvp in platforms)
                        {
                            var platformName = pfKvp.Key;
                            var platformData = pfKvp.Value as Dictionary<string, object> 
                                               ?? new Dictionary<string, object>();

                            var link = platformData.GetValueOrDefault("link")?.ToString();

                            // Create a bridging record in StreamingPlatform
                            // referencing the existingShow
                            var newSp = new StreamingPlatform
                            {
                                ServiceName = platformName,
                                Link = link,
                                StartDate = DateTime.UtcNow, // or parse from platformData
                                EndDate = null,
                                Show = existingShow // foreign key reference
                            };
                            db.StreamingPlatforms.Add(newSp);
                        }
                    }
                }
            }

            // 4) Save
            await db.SaveChangesAsync();
            return existingShow;
        }
    }
}
