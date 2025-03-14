using Microsoft.AspNetCore.Mvc;
using StreamBuddy.API.Services;
using StreamBuddy.API.Models;

namespace StreamBuddy.API.Controllers
{
    [Route("api/streaming")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        private readonly StreamingService _streamingService;

        public StreamingController(StreamingService streamingService)
        {
            _streamingService = streamingService;
        }

        [HttpGet("shows/search/title")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query, [FromQuery] string country = "us", [FromQuery] string showType = "movie")
        {
            try
            {
                var movies = await _streamingService.SearchMoviesAsync(query, country, showType);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("shows/top")]
        public async Task<IActionResult> GetTopShows(
            [FromQuery] string country = "us",
            [FromQuery] string services = "netflix") // Default to Netflix if not provided
        {
            try
            {
                var serviceList = services.Split(',').Select(s => s.Trim()).ToList(); // Convert CSV input to a list
                var movies = await _streamingService.GetTopShowsAsync(country, serviceList);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
