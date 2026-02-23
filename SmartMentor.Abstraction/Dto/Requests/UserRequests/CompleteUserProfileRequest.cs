using SmartMentor.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace SmartMentor.Abstraction.Dto.Requests.UserRequests
{
    public class CompleteUserProfileRequest
    {
        public List<UserSkillRequest> Skills { get; set; } = new List<UserSkillRequest>();
        [Required(ErrorMessage = "Interests are required")]
        public List<int> InterestIds { get; set; } = new List<int>();
        [Required(ErrorMessage = "Career goal is required")]
        public int CareerGoalId { get; set; }
    }

    public class UserSkillRequest
    {
        [Required(ErrorMessage = "SkillId is required")]
        public int SkillId { get; set; }
        [Required(ErrorMessage = "SkillLevel is required")]
        public SkillLevelEnum SkillLevel { get; set; }
    }
}
