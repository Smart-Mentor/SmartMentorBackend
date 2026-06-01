using SmartMentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Dto.Responses.UserResponse
{
    public class UserProfileResponseDto
    {
        public int CareerGoalId { get; set; }

        public string CareerGoalName { get; set; }

        public List<UserSkillDto> Skills { get; set; } = new();

        public List<UserInterestDto> Interests { get; set; } = new();

        // Messages
        public string? SkillsMessage { get; set; }

        public string? InterestsMessage { get; set; }

        public string? CareerGoalMessage { get; set; }
    }

    public class UserSkillDto
    {
        public int SkillId { get; set; }

        public string SkillName { get; set; }

        public SkillLevelEnum SkillLevel { get; set; }
    }

    public class UserInterestDto
    {
        public int InterestId { get; set; }

        public string InterestName { get; set; }
    }
}
