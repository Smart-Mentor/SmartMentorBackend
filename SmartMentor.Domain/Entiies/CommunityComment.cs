using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class CommunityComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Guid AuthorUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public CommunityPost Post { get; set; } = null!;
        public ApplicationUser Author { get; set; } = null!;
    }
}
