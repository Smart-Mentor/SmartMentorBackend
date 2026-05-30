using Microsoft.AspNetCore.Identity;
using SmartMentor.Domain.Entiies;

namespace SmartMentor.Persistence.Identity
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime? ProfileCompletedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int? CareerGoalId { get; set; }// Foreign key to CareerGoal
        public CareerGoal? CareerGoal { get; set; }
        public bool IsProfileCompleted { get; set; } = false;// This property indicates whether the user has completed their profile. It can be used to prompt users to complete their profile and to restrict access to certain features until the profile is completed.
        public ICollection<UserInterests>? UserInterests { get; set; } = new List<UserInterests>();
        public ICollection<UserSkills>? UserSkills { get; set; } = new List<UserSkills>();
        public ICollection<CommunityPost>? CommunityPosts { get; set; } = new List<CommunityPost>();
        public ICollection<CommunityComment>? CommunityComments { get; set; } = new List<CommunityComment>();
        public ICollection<CommunityPostReaction>? CommunityPostReactions { get; set; } = new List<CommunityPostReaction>();
    }
}
