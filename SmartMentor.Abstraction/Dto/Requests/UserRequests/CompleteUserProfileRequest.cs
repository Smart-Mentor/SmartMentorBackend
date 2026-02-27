using SmartMentor.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace SmartMentor.Abstraction.Dto.Requests.UserRequests
{
    public class CompleteUserProfileRequest
    {
        [Required(ErrorMessage = "Skills list cannot be null. Please provide at least one skill or an empty list.")]
        [MinLength(1, ErrorMessage = "At least one skill is required to complete your profile. Please add your skills with their proficiency levels.")]
        public List<UserSkillRequest> Skills { get; set; } = new List<UserSkillRequest>();
        
        [Required(ErrorMessage = "Interests are required. Please provide at least one interest ID.")]
        [MinLength(1, ErrorMessage = "You must select at least one area of interest to complete your profile.")]
        public List<int> InterestIds { get; set; } = new List<int>();
        
        [Required(ErrorMessage = "Career goal is required to complete your profile.")]
        [Range(1, int.MaxValue, ErrorMessage = "Career goal ID must be a valid positive number. Please select a valid career goal.")]
        public int CareerGoalId { get; set; }
    }

    public class UserSkillRequest
    {
        [Required(ErrorMessage = "Skill ID is required for each skill entry.")]
        [Range(1, int.MaxValue, ErrorMessage = "Skill ID must be a valid positive number. The provided skill ID is invalid.")]
        public int SkillId { get; set; }
        
        [Required(ErrorMessage = "Skill level is required. Please specify your proficiency level for this skill.")]
        public SkillLevelEnum SkillLevel { get; set; }
    }
}
