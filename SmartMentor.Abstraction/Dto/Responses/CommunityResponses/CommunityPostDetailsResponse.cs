namespace SmartMentor.Abstraction.Dto.Responses.CommunityResponses
{
    public class CommunityPostDetailsResponse
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CommunityAuthorResponse Author { get; set; } = new();
        public CommunityCareerGoalTagResponse PrimaryCareerGoal { get; set; } = new();
        public List<CommunityCareerGoalTagResponse> Tags { get; set; } = new();
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<CommunityCommentResponse> Comments { get; set; } = new();
    }

    public class CommunityCommentResponse
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CommunityAuthorResponse Author { get; set; } = new();
    }
}
