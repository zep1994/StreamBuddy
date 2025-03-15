// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace StreamBuddy.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        // Navigation property: A user can have many reactions
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
