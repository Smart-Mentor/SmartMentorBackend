using System.ComponentModel.DataAnnotations;

namespace SmartMentor.Abstraction.Dto.Requests.CommunityRequests
{
    public class CreateCommunityCommentRequest
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }
}
