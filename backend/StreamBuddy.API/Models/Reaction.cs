// Models/Reaction.cs
using System.ComponentModel.DataAnnotations;

namespace StreamBuddy.API.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ReactionType { get; set; } = string.Empty;

        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
