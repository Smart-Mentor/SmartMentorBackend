using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.CommunityRequests
{
    public class CreateCommunityPostRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int PrimaryCareerGoalId { get; set; }

        public List<int> CareerGoalTagIds { get; set; } = new();
    }
}
