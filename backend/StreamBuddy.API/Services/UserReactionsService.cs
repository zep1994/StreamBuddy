// Services/UserReactionsService.cs
using Microsoft.EntityFrameworkCore;
using StreamBuddy.API.Data;
using StreamBuddy.API.Models.Users;

namespace StreamBuddy.API.Services
{
    public class UserReactionsService
    {
        private readonly AppDbContext _context;

        public UserReactionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> ReactToMovieAsync(int userId, int movieId, string reactionType)
        {
            // Check if the movie exists
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return "Movie not found.";
            }

            // Check if a reaction already exists for this user and movie
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (existingReaction != null)
            {
                // Update the reaction
                existingReaction.ReactionType = reactionType;
            }
            else
            {
                // Create a new reaction
                var reaction = new Reaction
                {
                    UserId = userId,
                    MovieId = movieId,
                    ReactionType = reactionType
                };

                _context.Reactions.Add(reaction);
            }

            await _context.SaveChangesAsync();
            return "Reaction saved successfully.";
        }
    }
}
