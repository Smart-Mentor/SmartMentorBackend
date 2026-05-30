namespace SmartMentor.Abstraction.Dto.Responses.CommunityResponses
{
    public class CommunityPostSummaryResponse
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public CommunityAuthorResponse Author { get; set; } = new();
        public CommunityCareerGoalTagResponse PrimaryCareerGoal { get; set; } = new();
        public List<CommunityCareerGoalTagResponse> Tags { get; set; } = new();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    public class CommunityAuthorResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class CommunityCareerGoalTagResponse
    {
        public int CareerGoalId { get; set; }
        public string CareerGoalName { get; set; } = string.Empty;
    }
}
