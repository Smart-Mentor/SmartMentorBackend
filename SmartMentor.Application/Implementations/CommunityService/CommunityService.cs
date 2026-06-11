using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.CommunityRequests;
using SmartMentor.Abstraction.Dto.Responses.CommunityResponses;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.CommunityService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Application.Implementations.CommunityService
{
    public class CommunityService : ICommunityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CommunityService> _logger;

        public CommunityService(IUnitOfWork unitOfWork, 
        UserManager<ApplicationUser> userManager,
        ILogger<CommunityService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CommunityPostSummaryResponse>> GetPostsByCareerGoalAsync(int careerGoalId, Guid currentUserId, CancellationToken cancellationToken = default)
        {
            var careerGoalExists = await _unitOfWork.Repository<CareerGoal>()
                .AnyAsync(cg => cg.Id == careerGoalId, cancellationToken);

            if (!careerGoalExists)
            {
                throw new KeyNotFoundException($"Career goal with id {careerGoalId} not found.");
            }

            var primaryPosts = await _unitOfWork.Repository<CommunityPost>()
                .FindAsync(p => !p.IsDeleted && p.PrimaryCareerGoalId == careerGoalId, cancellationToken, p => p.Author, p => p.PrimaryCareerGoal);
            var taggedPostLinks = await _unitOfWork.Repository<CommunityPostCareerGoalTag>()
                .FindAsync(t => t.CareerGoalId == careerGoalId, cancellationToken, t => t.Post);

            var postIds = primaryPosts.Select(p => p.Id)
                .Union(taggedPostLinks.Select(t => t.PostId))
                .Distinct()
                .ToList();

            var posts = await _unitOfWork.Repository<CommunityPost>()
                .FindAsync(p => postIds.Contains(p.Id) && !p.IsDeleted, cancellationToken, p => p.Author, p => p.PrimaryCareerGoal);
            posts = posts.OrderByDescending(p => p.CreatedAt).ToList();

            var tagMap = await GetTagMapAsync(postIds, cancellationToken);
            var likeMap = await GetLikeCountMapAsync(postIds, cancellationToken);
            var commentMap = await GetCommentCountMapAsync(postIds, cancellationToken);
            var likedPostIds = (await _unitOfWork.Repository<CommunityPostReaction>()
                .FindAsync(r => r.UserId == currentUserId && postIds.Contains(r.PostId), cancellationToken))
                .Select(r => r.PostId)
                .ToList();

            return posts.Select(post => new CommunityPostSummaryResponse
            {
                PostId = post.Id,
                Title = post.Title,
                ContentPreview = post.Content.Length <= 200 ? post.Content : $"{post.Content[..200]}...",
                CreatedAt = post.CreatedAt,
                Author = MapAuthor(post.Author),
                PrimaryCareerGoal = MapCareerGoal(post.PrimaryCareerGoal),
                Tags = tagMap.GetValueOrDefault(post.Id, new List<CommunityCareerGoalTagResponse>()),
                LikeCount = likeMap.GetValueOrDefault(post.Id, 0),
                CommentCount = commentMap.GetValueOrDefault(post.Id, 0),
                IsLikedByCurrentUser = likedPostIds.Contains(post.Id)
            }).ToList();
        }

        public async Task<CommunityPostDetailsResponse> GetPostByIdAsync(int postId, Guid currentUserId, CancellationToken cancellationToken = default)
        {
            var post = (await _unitOfWork.Repository<CommunityPost>()
                .FindAsync(p => p.Id == postId && !p.IsDeleted, cancellationToken, p => p.Author, p => p.PrimaryCareerGoal))
                .FirstOrDefault();

            if (post == null)
            {
                throw new KeyNotFoundException($"Post with id {postId} not found.");
            }

            var comments = (await _unitOfWork.Repository<CommunityComment>()
                .FindAsync(c => c.PostId == postId && !c.IsDeleted, cancellationToken, c => c.Author))
                .OrderBy(c => c.CreatedAt)
                .ToList();

            var tags = await GetTagMapAsync(new List<int> { postId }, cancellationToken);
            var likeCount = await _unitOfWork.Repository<CommunityPostReaction>()
                .CountAsync(r => r.PostId == postId, cancellationToken);
            var isLikedByCurrentUser = await _unitOfWork.Repository<CommunityPostReaction>()
                .AnyAsync(r => r.PostId == postId && r.UserId == currentUserId, cancellationToken);

            return new CommunityPostDetailsResponse
            {
                PostId = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Author = MapAuthor(post.Author),
                PrimaryCareerGoal = MapCareerGoal(post.PrimaryCareerGoal),
                Tags = tags.GetValueOrDefault(post.Id, new List<CommunityCareerGoalTagResponse>()),
                LikeCount = likeCount,
                IsLikedByCurrentUser = isLikedByCurrentUser,
                Comments = comments.Select(comment => new CommunityCommentResponse
                {
                    CommentId = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    Author = MapAuthor(comment.Author)
                }).ToList()
            };
        }

        public async Task<CommunityPostDetailsResponse> CreatePostAsync(Guid userId, CreateCommunityPostRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId);
            await ValidateCareerGoalIdsAsync(request.PrimaryCareerGoalId, request.CareerGoalTagIds, cancellationToken);

            var post = new CommunityPost
            {
                AuthorUserId = userId,
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                PrimaryCareerGoalId = request.PrimaryCareerGoalId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Repository<CommunityPost>().AddAsync(post, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var normalizedTagIds = request.CareerGoalTagIds
                .Append(request.PrimaryCareerGoalId)
                .Distinct()
                .ToList();

            if (normalizedTagIds.Any())
            {
                var tags = normalizedTagIds.Select(tagId => new CommunityPostCareerGoalTag
                {
                    PostId = post.Id,
                    CareerGoalId = tagId
                });

                await _unitOfWork.Repository<CommunityPostCareerGoalTag>().AddRangeAsync(tags, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return await GetPostByIdAsync(post.Id, userId, cancellationToken);
        }

        public async Task<CommunityCommentResponse> AddCommentAsync(Guid userId, int postId, CreateCommunityCommentRequest request, CancellationToken cancellationToken = default)
        {
            var user = await EnsureUserExistsAsync(userId);
            await EnsurePostExistsAsync(postId, cancellationToken);

            var comment = new CommunityComment
            {
                PostId = postId,
                AuthorUserId = userId,
                Content = request.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Repository<CommunityComment>().AddAsync(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CommunityCommentResponse
            {
                CommentId = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Author = MapAuthor(user)
            };
        }

        public async Task AddLikeAsync(Guid userId, int postId, CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId);
            await EnsurePostExistsAsync(postId, cancellationToken);

            var exists = await _unitOfWork.Repository<CommunityPostReaction>()
                .AnyAsync(r => r.PostId == postId && r.UserId == userId, cancellationToken);

            if (exists)
            {
                return;
            }

            await _unitOfWork.Repository<CommunityPostReaction>().AddAsync(new CommunityPostReaction
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveLikeAsync(Guid userId, int postId, CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId);
            await EnsurePostExistsAsync(postId, cancellationToken);

            var reaction = (await _unitOfWork.Repository<CommunityPostReaction>()
                .FindAsync(r => r.PostId == postId && r.UserId == userId, cancellationToken))
                .FirstOrDefault();

            if (reaction == null)
            {
                return;
            }

            _unitOfWork.Repository<CommunityPostReaction>().Delete(reaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<ApplicationUser> EnsureUserExistsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {userId} not found.");
            }

            return user;
        }

        private async Task EnsurePostExistsAsync(int postId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<CommunityPost>()
                .AnyAsync(p => p.Id == postId && !p.IsDeleted, cancellationToken);

            if (!exists)
            {
                throw new KeyNotFoundException($"Post with id {postId} not found.");
            }
        }

        private async Task ValidateCareerGoalIdsAsync(int primaryCareerGoalId, IEnumerable<int> tagIds, CancellationToken cancellationToken)
        {
            var requestedIds = tagIds.Append(primaryCareerGoalId).Distinct().ToList();
            var validIds = (await _unitOfWork.Repository<CareerGoal>()
                .FindAsync(cg => requestedIds.Contains(cg.Id), cancellationToken))
                .Select(cg => cg.Id)
                .ToList();

            var invalidIds = requestedIds.Except(validIds).ToList();
            if (invalidIds.Any())
            {
                throw new KeyNotFoundException($"Career goal ids not found: {string.Join(", ", invalidIds)}");
            }
        }

        private async Task<Dictionary<int, List<CommunityCareerGoalTagResponse>>> GetTagMapAsync(List<int> postIds, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.Repository<CommunityPostCareerGoalTag>()
                .FindAsync(t => postIds.Contains(t.PostId), cancellationToken, t => t.CareerGoal);

            return tags
                .GroupBy(t => t.PostId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(t => MapCareerGoal(t.CareerGoal)).OrderBy(t => t.CareerGoalName).ToList());
        }

        private async Task<Dictionary<int, int>> GetLikeCountMapAsync(List<int> postIds, CancellationToken cancellationToken)
        {
            return (await _unitOfWork.Repository<CommunityPostReaction>()
                .FindAsync(r => postIds.Contains(r.PostId), cancellationToken))
                .GroupBy(r => r.PostId)
                .Select(group => new { group.Key, Count = group.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        private async Task<Dictionary<int, int>> GetCommentCountMapAsync(List<int> postIds, CancellationToken cancellationToken)
        {
            return (await _unitOfWork.Repository<CommunityComment>()
                .FindAsync(c => postIds.Contains(c.PostId) && !c.IsDeleted, cancellationToken))
                .GroupBy(c => c.PostId)
                .Select(group => new { group.Key, Count = group.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        private static CommunityAuthorResponse MapAuthor(ApplicationUser user)
        {
            return new CommunityAuthorResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        private static CommunityCareerGoalTagResponse MapCareerGoal(CareerGoal careerGoal)
        {
            return new CommunityCareerGoalTagResponse
            {
                CareerGoalId = careerGoal.Id,
                CareerGoalName = careerGoal.Name
            };
        }

        public async Task DeletePostAsync(Guid userId, int postId, CancellationToken cancellationToken = default)
        {
           var ISPostExists = await _unitOfWork.Repository<CommunityPost>()
                .AnyAsync(p => p.Id == postId && !p.IsDeleted, cancellationToken);

            if (!ISPostExists)
            {
                _logger.LogWarning("Attempt to delete non-existent post with id {PostId}", postId);
                throw new KeyNotFoundException($"Post with id {postId} not found.");
            }

            var post = await _unitOfWork.Repository<CommunityPost>()
                .FindAsync(p => p.Id == postId && !p.IsDeleted, cancellationToken);

            var postAuthorId = post.FirstOrDefault();
            if (post == null || postAuthorId == null)
            {
                _logger.LogWarning("Attempt to delete non-existent post with id {PostId}", postId);
                throw new KeyNotFoundException($"Post with id {postId} not found.");
            }
            if(postAuthorId.AuthorUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete post {PostId} they do not own", userId, postId);
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");
            }
            postAuthorId.IsDeleted = true;
            _unitOfWork.Repository<CommunityPost>().Update(postAuthorId);
            await _unitOfWork.SaveChangesAsync(cancellationToken); 
        }

    }
}
