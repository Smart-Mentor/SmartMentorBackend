namespace SmartMentorApi.Controllers.UserController
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SmartMentor.Abstraction.Dto.Requests.UserRequests;
    using SmartMentor.Abstraction.Dto.SharedRequestsAndResponses;
    using SmartMentor.Abstraction.Services.CompleteUserProfileService;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteUserProfileRequest request, CancellationToken cancellationToken)
        {
            // Validate user authentication
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    Success = false,
                    Message = "Authentication failed. User ID not found in token.",
                    ErrorCode = "AUTH_001",
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "Authorization",
                            Message = "You must be logged in to complete your profile. Please provide a valid authentication token."
                        }
                    }
                });
            }

            // Validate model state
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new ErrorDetail
                    {
                        Field = x.Key,
                        Message = e.ErrorMessage
                    }))
                    .ToList();

                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Validation failed. Please check the provided data and try again.",
                    ErrorCode = "VALIDATION_001",
                    Errors = validationErrors
                });
            }

            // Execute service call
            var result = await _userProfileService.CompleteAsync(Guid.Parse(userId), request, cancellationToken);
            
            if (result.IsSuccess)
            {
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "User profile completed successfully.",
                    Data = new
                    {
                        UserId = userId,
                        ProfileCompletedAt = DateTime.UtcNow
                    }
                });
            }
            else
            {
                var errors = result.Errors.Select(e => new ErrorDetail
                {
                    Field = e.Metadata.ContainsKey("PropertyName") ? e.Metadata["PropertyName"]?.ToString() ?? "General" : "General",
                    Message = e.Message,
                    Code = e.Metadata.ContainsKey("ErrorCode") ? e.Metadata["ErrorCode"]?.ToString() : null
                }).ToList();

                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Failed to complete user profile. Please review the errors below.",
                    ErrorCode = "PROFILE_001",
                    Errors = errors
                });
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] CompleteUserProfileRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _userProfileService
                .UpdateAsync(userId, request, cancellationToken);

            if (result.IsSuccess)
                return Ok(new { Message = "User profile updated successfully." });

            return BadRequest(result.Errors.Select(e => e.Message));
        }
        [HttpPatch("update-skill-level/{skillId}")]
        public async Task<IActionResult> UpdateSkillLevel(int skillId,CancellationToken cancellationToken)
        {
            var userId=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)            {
                return Unauthorized("User ID not found in token.");
            }
            var result = await _userProfileService.updateSkillLevel(Guid.Parse(userId), skillId, cancellationToken);
            if (result.IsSuccess)
            {
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Skill level updated successfully.",
                    Data = new
                    {
                        UserId=userId,
                        SkillId=skillId,
                        UpdatedAt = DateTime.UtcNow
                    }
                });
            }
            else
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Failed to update skill level.",
                    ErrorCode = "SKILL_001",
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "SkillLevel",
                            Message = result.Errors.FirstOrDefault()?.Message ?? "Unknown error occurred."
                        }
                    }
                });
            }
        }
    }
}




