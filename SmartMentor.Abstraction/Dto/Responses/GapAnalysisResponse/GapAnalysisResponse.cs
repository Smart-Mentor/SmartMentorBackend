using SmartMentor.Domain.Enums;

namespace SmartMentor.Abstraction.Dto.Responses.GapAnalysisResponse
{
    public class GapAnalysisResponse
    {
        public string CareerGoalName { get; set; } = string.Empty;
        public List<SkillGapItem> MissingSkills { get; set; } = new List<SkillGapItem>();
        public List<SkillGapItem> ReadySkills { get; set; } = new List<SkillGapItem>();

        public List<SkillGapItem> WeakSkills { get; set; } = new List<SkillGapItem>();


    }

    public class SkillGapItem
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public SkillLevelEnum RequiredLevel { get; set; }
        public SkillLevelEnum? CurrentLevel { get; set; }
        
    }
}