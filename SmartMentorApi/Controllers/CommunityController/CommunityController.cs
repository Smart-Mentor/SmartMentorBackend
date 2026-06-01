using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Dto.Requests.CommunityRequests;
using SmartMentor.Abstraction.Dto.SharedRequestsAndResponses;
using SmartMentor.Abstraction.Services.CommunityService;

namespace SmartMentorApi.Controllers.CommunityController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly ILogger<CommunityController> _logger;

        public CommunityController(ICommunityService communityService, ILogger<CommunityController> logger)
        {
            _communityService = communityService;
            _logger = logger;
        }

        [HttpGet("career-goals/{careerGoalId}/posts")]
        public async Task<IActionResult> GetPostsByCareerGoal(int careerGoalId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            try
            {
                var posts = await _communityService.GetPostsByCareerGoalAsync(careerGoalId, currentUserId, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Community posts retrieved successfully.",
                    Data = posts
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Career goal {CareerGoalId} not found while retrieving posts", careerGoalId);
                return NotFound(BuildNotFoundResponse("careerGoalId", $"Career goal with id {careerGoalId} not found.", "COMMUNITY_404"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving community posts for career goal {CareerGoalId}", careerGoalId);
                return StatusCode(500, BuildServerErrorResponse("An error occurred while retrieving community posts."));
            }
        }

        [HttpGet("posts/{postId}")]
        public async Task<IActionResult> GetPostById(int postId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            try
            {
                var post = await _communityService.GetPostByIdAsync(postId, currentUserId, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Community post retrieved successfully.",
                    Data = post
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Post {PostId} not found", postId);
                return NotFound(BuildNotFoundResponse("postId", $"Post with id {postId} not found.", "COMMUNITY_404"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving community post {PostId}", postId);
                return StatusCode(500, BuildServerErrorResponse("An error occurred while retrieving the community post."));
            }
        }

        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromBody] CreateCommunityPostRequest request, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(BuildValidationResponse());
            }

            try
            {
                var post = await _communityService.CreatePostAsync(currentUserId, request, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Community post created successfully.",
                    Data = post
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Community post creation failed due to missing related data");
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "COMMUNITY_404",
                    Errors = new List<ErrorDetail>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating community post");
                return StatusCode(500, BuildServerErrorResponse("An error occurred while creating the community post."));
            }
        }

        [HttpPost("posts/{postId}/comments")]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CreateCommunityCommentRequest request, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(BuildValidationResponse());
            }

            try
            {
                var comment = await _communityService.AddCommentAsync(currentUserId, postId, request, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Comment added successfully.",
                    Data = comment
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Comment creation failed because post {PostId} was not found", postId);
                return NotFound(BuildNotFoundResponse("postId", $"Post with id {postId} not found.", "COMMUNITY_404"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to post {PostId}", postId);
                return StatusCode(500, BuildServerErrorResponse("An error occurred while adding the comment."));
            }
        }

        [HttpPost("posts/{postId}/like")]
        public async Task<IActionResult> LikePost(int postId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            try
            {
                await _communityService.AddLikeAsync(currentUserId, postId, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Post liked successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Like failed because post {PostId} was not found", postId);
                return NotFound(BuildNotFoundResponse("postId", $"Post with id {postId} not found.", "COMMUNITY_404"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking post {PostId}", postId);
                return StatusCode(500, BuildServerErrorResponse("An error occurred while liking the post."));
            }
        }

        [HttpDelete("posts/{postId}/like")]
        public async Task<IActionResult> RemoveLike(int postId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(BuildUnauthorizedResponse());
            }

            try
            {
                await _communityService.RemoveLikeAsync(currentUserId, postId, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Post like removed successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unlike failed because post {PostId} was not found", postId);
                return NotFound(BuildNotFoundResponse("postId", $"Post with id {postId} not found.", "COMMUNITY_404"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing like from post {PostId}", postId);
                return StatusCode(500, BuildServerErrorResponse("An error occurred while removing the post like."));
            }
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out userId);
        }

        private ErrorResponse BuildUnauthorizedResponse()
        {
            return new ErrorResponse
            {
                Success = false,
                Message = "Authentication failed. User ID not found in token.",
                ErrorCode = "AUTH_001",
                Errors = new List<ErrorDetail>()
            };
        }

        private ErrorResponse BuildValidationResponse()
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new ErrorDetail
                {
                    Field = x.Key,
                    Message = e.ErrorMessage
                }))
                .ToList();

            return new ErrorResponse
            {
                Success = false,
                Message = "Validation failed. Please check the provided data and try again.",
                ErrorCode = "VALIDATION_001",
                Errors = validationErrors
            };
        }

        private ErrorResponse BuildNotFoundResponse(string field, string message, string errorCode)
        {
            return new ErrorResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = field,
                        Message = message
                    }
                }
            };
        }

        private ErrorResponse BuildServerErrorResponse(string message)
        {
            return new ErrorResponse
            {
                Success = false,
                Message = message,
                ErrorCode = "COMMUNITY_500",
                Errors = new List<ErrorDetail>()
            };
        }
    }
}
