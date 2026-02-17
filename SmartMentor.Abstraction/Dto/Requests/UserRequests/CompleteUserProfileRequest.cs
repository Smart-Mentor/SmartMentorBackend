using SmartMentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Abstraction.Dto.Requests.UserRequests
{
    public class CompleteUserProfileRequest
    {
        public List<UserSkillRequest> Skills { get; set; } = new List<UserSkillRequest>();
        public List<int> InterestIds { get; set; } = new List<int>();
        public int CareerGoalId { get; set; }
        public string? CarrerGoadDescription { get; set; }
    }

    public class UserSkillRequest
    {
        public int SkillId { get; set; }
        public SkillLevelEnum SkillLevel { get; set; }
    }
}
