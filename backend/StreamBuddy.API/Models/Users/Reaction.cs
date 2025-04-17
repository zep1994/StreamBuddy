using System.ComponentModel.DataAnnotations;
using StreamBuddy.API.Models.Movies;
using StreamBuddy.API.Models.Shows;

namespace StreamBuddy.API.Models.Users
{
    public class Reaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int? ShowId { get; set; }
        public Show? Show { get; set; }

        public string ReactionType { get; set; } = "Like";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}