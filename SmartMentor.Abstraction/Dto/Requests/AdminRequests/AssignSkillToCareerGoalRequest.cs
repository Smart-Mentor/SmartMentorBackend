using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.AdminRequests
{
    public class AssignSkillToCareerGoalRequest
    {
        [Required(ErrorMessage = "Career Goal Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Career Goal Id must be greater than 0")]
        public int CareerGoalId { get; set; }

        [Required(ErrorMessage = "Skill Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Skill Id must be greater than 0")]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Required Level is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required Level must be greater than 0")]
        public int RequiredLevel { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Priority must be greater than 0")]
        public int Priority { get; set; }
    }
}