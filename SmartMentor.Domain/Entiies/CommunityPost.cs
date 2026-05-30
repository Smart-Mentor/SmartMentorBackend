using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    public class CommunityPost
    {
        public int Id { get; set; }
        public Guid AuthorUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int PrimaryCareerGoalId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ApplicationUser Author { get; set; } = null!;
        public CareerGoal PrimaryCareerGoal { get; set; } = null!;
        public ICollection<CommunityComment> Comments { get; set; } = new List<CommunityComment>();
        public ICollection<CommunityPostCareerGoalTag> CareerGoalTags { get; set; } = new List<CommunityPostCareerGoalTag>();
        public ICollection<CommunityPostReaction> Reactions { get; set; } = new List<CommunityPostReaction>();
    }
}
