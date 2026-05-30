namespace SmartMentor.Domain.Entiies
{
    public class CommunityPostCareerGoalTag
    {
        public int PostId { get; set; }
        public int CareerGoalId { get; set; }

        public CommunityPost Post { get; set; } = null!;
        public CareerGoal CareerGoal { get; set; } = null!;
    }
}
