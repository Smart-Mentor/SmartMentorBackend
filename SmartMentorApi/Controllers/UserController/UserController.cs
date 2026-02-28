namespace SmartMentorApi.Controllers.UserController
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SmartMentor.Abstraction.Dto.Requests.UserRequests;
    using SmartMentor.Abstraction.Services.CompleteUserProfileService.cs;
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
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userProfileService.CompleteAsync(Guid.Parse(userId), request, cancellationToken);
            if (result.IsSuccess)
            {
                return Ok(new { Message = "User profile completed successfully." });
            }
            else
            {
                return BadRequest(result.Errors.Select(e => e.Message));
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
    }
}