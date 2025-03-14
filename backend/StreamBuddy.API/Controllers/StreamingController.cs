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

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            try
            {
                var movies = await _streamingService.SearchMoviesAsync(query);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("platform")]
        public async Task<IActionResult> GetMoviesByPlatform([FromQuery] string platform)
        {
            try
            {
                var movies = await _streamingService.GetMoviesByPlatformAsync(platform);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
