using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/streaming")]
public class StreamingController : ControllerBase
{
    private readonly StreamingService _streamingService;

    public StreamingController(StreamingService streamingService)
    {
        _streamingService = streamingService;
    }

    [HttpGet("availability/{type}/{id}")]
    public async Task<IActionResult> GetShowAvailability(string type, string id, [FromQuery] string? country = "us")
    {
        try
        {
            var result = await _streamingService.GetShowAvailability(type, id, country);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { message = "Error fetching data from RapidAPI", error = ex.Message });
        }
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchMovies(string query, [FromQuery] string? country = "us")
    {
        try
        {
            var result = await _streamingService.SearchMovies(query, country);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { message = "Error fetching data from RapidAPI", error = ex.Message });
        }
    }
}
