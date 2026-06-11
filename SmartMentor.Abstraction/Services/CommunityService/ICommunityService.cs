using SmartMentor.Abstraction.Dto.Requests.CommunityRequests;
using SmartMentor.Abstraction.Dto.Responses.CommunityResponses;

namespace SmartMentor.Abstraction.Services.CommunityService
{
    public interface ICommunityService
    {
        Task<IReadOnlyList<CommunityPostSummaryResponse>> GetPostsByCareerGoalAsync(int careerGoalId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<CommunityPostDetailsResponse> GetPostByIdAsync(int postId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<CommunityPostDetailsResponse> CreatePostAsync(Guid userId, CreateCommunityPostRequest request, CancellationToken cancellationToken = default);
        Task<CommunityCommentResponse> AddCommentAsync(Guid userId, int postId, CreateCommunityCommentRequest request, CancellationToken cancellationToken = default);
        Task AddLikeAsync(Guid userId, int postId, CancellationToken cancellationToken = default);
        Task RemoveLikeAsync(Guid userId, int postId, CancellationToken cancellationToken = default);
        Task DeletePostAsync(Guid userId, int postId, CancellationToken cancellationToken = default);
    }
}
