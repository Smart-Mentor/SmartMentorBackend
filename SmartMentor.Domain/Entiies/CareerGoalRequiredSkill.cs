using SmartMentor.Domain.Enums;

namespace SmartMentor.Domain.Entiies
{

    public class CareerGoalRequiredSkill
    {
        public int CareerGoalId { get; set; }
        public int SkillId { get; set; }

        public SkillLevelEnum RequiredLevel { get; set; }

        public int? Priority { get; set; }

        public CareerGoal CareerGoal { get; set; }
        public Skill Skill { get; set; }
    }
}
