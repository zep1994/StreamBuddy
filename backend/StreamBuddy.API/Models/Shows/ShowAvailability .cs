
using StreamBuddy.API.Models.Platforms;

namespace StreamBuddy.API.Models.Shows
{
    public class ShowAvailability
    {
        public int Id { get; set; }

        // Foreign keys to Show and StreamingPlatform
        public int ShowId { get; set; }
        public Show? Show { get; set; }

        public int PlatformId { get; set; }
        public StreamingPlatform? Platform { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
