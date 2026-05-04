namespace SmartMentor.Abstraction.Dto.Responses.AdminResponse
{
    public class MasterDataResponse
    {
        public IEnumerable<SkillResponse> Skills { get; set; }
        public IEnumerable<InterestResponse> Interests { get; set; }
        public IEnumerable<CareerGoalResponse> CareerGoals { get; set; }
    }
}