using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class CommunityPostReaction
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CommunityPost Post { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
