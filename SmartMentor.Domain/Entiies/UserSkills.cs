using SmartMentor.Domain.Enums;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Domain.Entiies
{
    // This class represents the skills that a user has. It can be used to match mentors and mentees based on their skills and to recommend mentors or mentees with similar skills.
    // For example, if a user has a skill in "C#", they can be matched
    public class UserSkills
    {
        public Guid UserId { get; set; }// Foreign key to ApplicationUser
        public int SkillId { get; set; }// Foreign key to Skill
        public SkillLevelEnum SkillLevel { get; set; } // e.g., Beginner, Intermediate, Expert
        public ApplicationUser User { get; set; }
        public Skill Skill { get; set; }
    }
}