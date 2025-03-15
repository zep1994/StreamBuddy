// Controllers/UserReactionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamBuddy.API.Services;
using System.Security.Claims;

namespace StreamBuddy.API.Controllers
{
    [Route("api/reactions")]
    [ApiController]
    [Authorize]
    public class UserReactionsController : ControllerBase
    {
        private readonly UserReactionsService _userReactionsService;

        public UserReactionsController(UserReactionsService userReactionsService)
        {
            _userReactionsService = userReactionsService;
        }

        [HttpPost("{movieId}")]
        public async Task<IActionResult> ReactToMovie(int movieId, [FromBody] ReactionDto reactionDto)
        {
            // Get the current user id from the token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var result = await _userReactionsService.ReactToMovieAsync(userId, movieId, reactionDto.ReactionType);

            if(result == "Movie not found.")
                return NotFound(result);

            return Ok(result);
        }
    }

    public class ReactionDto
    {
        // Expected values: "like", "love"
        public string ReactionType { get; set; } = string.Empty;
    }
}
