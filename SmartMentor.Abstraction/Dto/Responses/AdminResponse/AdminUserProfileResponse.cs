using SmartMentor.Abstraction.Dto.Responses.UserResponse;

namespace SmartMentor.Abstraction.Dto.Responses.AdminResponse
{
    public class AdminUserProfileResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? CareerGoalId { get; set; }
        public string CareerGoalName { get; set; } = string.Empty;
        public string? CareerGoalMessage { get; set; }
        public List<UserSkillDto> Skills { get; set; } = new();
        public List<UserInterestDto> Interests { get; set; } = new();
        public string? SkillsMessage { get; set; }
        public string? InterestsMessage { get; set; }
    }
}
