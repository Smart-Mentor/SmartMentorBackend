using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AdminAnalyticsService;
using SmartMentor.Abstraction.Services.AdminService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;
using Microsoft.AspNetCore.Authorization;
using SmartMentor.Abstraction.Dto.SharedRequestsAndResponses;

namespace SmartMentorApi.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminAnalyticsService _adminAnalyticsService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService,
         IAdminAnalyticsService adminAnalyticsService,
         IUnitOfWork unitOfWork,
         ILogger<AdminController> logger
         )
        {
            _adminService = adminService;
            _adminAnalyticsService = adminAnalyticsService;
            _logger = logger;
        }
        [HttpGet("analytics/overview")]
        public async Task<IActionResult> GetAnalyticsOverview(CancellationToken cancellationToken)
        {
            try
            {
                var overview = await _adminAnalyticsService.GetOverviewAsync(cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Admin analytics overview retrieved successfully.",
                    Data = overview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin analytics overview");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving admin analytics overview.",
                    ErrorCode = "ANALYTICS_500",
                    Errors = new List<ErrorDetail>()
                });
            }
        }
        [HttpGet("analytics/user-growth")]
        public async Task<IActionResult> GetUserGrowth(
            [FromQuery] string groupBy = "day",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var growth = await _adminAnalyticsService.GetUserGrowthAsync(groupBy, startDate, endDate, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "User growth analytics retrieved successfully.",
                    Data = growth
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Validation failed for analytics query.",
                    ErrorCode = "VALIDATION_003",
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "analyticsQuery",
                            Message = ex.Message
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user growth analytics");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving user growth analytics.",
                    ErrorCode = "ANALYTICS_500",
                    Errors = new List<ErrorDetail>()
                });
            }
        }
        [HttpGet("analytics/onboarding-funnel")]
        public async Task<IActionResult> GetOnboardingFunnel(CancellationToken cancellationToken)
        {
            try
            {
                var funnel = await _adminAnalyticsService.GetOnboardingFunnelAsync(cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Onboarding funnel analytics retrieved successfully.",
                    Data = funnel
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving onboarding funnel analytics");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving onboarding funnel analytics.",
                    ErrorCode = "ANALYTICS_500",
                    Errors = new List<ErrorDetail>()
                });
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync(cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }
        [HttpGet("users/profile-summaries")]
        public async Task<IActionResult> GetAllUsersProfileSummaries(CancellationToken cancellationToken)
        {
            try
            {
                var userProfileSummaries = await _adminService.GetAllUsersProfileSummariesAsync(cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "User skills, interests, and career goal retrieved successfully.",
                    Data = userProfileSummaries
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile summaries");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving user profile summaries.",
                    ErrorCode = "USER_500",
                    Errors = new List<ErrorDetail>()
                });
            }
        }
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(userId, cancellationToken);
                if (user == null)
                {
                    return NotFound($"User with id {userId} not found.");
                }
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", userId);
                return NotFound($"User with id {userId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with id {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }
        [HttpGet("users/{userId}/profile-summary")]
        public async Task<IActionResult> GetUserSkillsAndInterests([FromRoute] string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Validation failed. The provided user id is not a valid GUID.",
                    ErrorCode = "VALIDATION_002",
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "userId",
                            Message = "The userId route parameter must be a valid GUID."
                        }
                    }
                });
            }

            try
            {
                var userProfileSummary = await _adminService.GetUserSkillsAndInterestsAsync(parsedUserId, cancellationToken);
                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "User skills, interests, and career goal retrieved successfully.",
                    Data = userProfileSummary
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", parsedUserId);
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = $"User with id {parsedUserId} not found.",
                    ErrorCode = "USER_404",
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail
                        {
                            Field = "userId",
                            Message = $"No user exists with id {parsedUserId}."
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skills and interests for user with id {UserId}", parsedUserId);
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user profile summary.",
                    ErrorCode = "USER_500",
                    Errors = new List<ErrorDetail>()
                });
            }
        }
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.DeleteUserAsync(userId, cancellationToken);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", userId);
                return NotFound($"User with id {userId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id {UserId}", userId);
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _adminService.GetUserRolesAsync(userId, cancellationToken);
                return Ok(roles);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", userId);
                return NotFound($"User with id {userId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user with id {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user roles.");
            }
        }
        [HttpPost("users/assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.AssignRoleToUserAsync(request, cancellationToken);
                if (result)
                {
                    return Ok("Role assigned to user successfully.");
                }
                return BadRequest("Failed to assign role to user. Please check if the user and role exist.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", request.UserId);
                return NotFound($"User with id {request.UserId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleName} to user with id {UserId}", request.RoleName, request.UserId);
                return StatusCode(500, "An error occurred while assigning role to user.");
            }
        }
            [HttpPost("users/remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.RemoveRoleFromUserAsync(request, cancellationToken);
                if (result)
                {
                    return Ok("Role removed from user successfully.");
                }
                return BadRequest("Failed to remove role from user. Please check if the user and role exist.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User with id {UserId} not found", request.UserId);
                return NotFound($"User with id {request.UserId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleName} from user with id {UserId}", request.RoleName, request.UserId);
                return StatusCode(500, "An error occurred while removing role from user.");
            }
        }
        [HttpGet("skills")]
        public async Task<IActionResult> GetSkills(CancellationToken cancellationToken)
        {
            try
            {
                var skills = await _adminService.GetAllSkillsAsync(cancellationToken);

                var projected = skills.Select(s => new {
                    id = s.Id,
                    name = s.Name,
                    category = s.Category
                });

                return Ok(projected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skills.");
                return StatusCode(500, "An error occurred while retrieving skills.");
            }
        }
        [HttpPost("skills")]
        public async Task<IActionResult> CreateSkill([FromBody] AddSkillRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.CreateSkillAsync(request, cancellationToken);

                return Ok("Skill created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating skill.");
                return StatusCode(500, "An error occurred while creating the skill.");
            }
        }
        [HttpPut("skills/{skillId}")]
        public async Task<IActionResult> UpdateSkill(int skillId, [FromBody] AddSkillRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var updatedSkill = await _adminService.UpdateSkillAsync(skillId, request, cancellationToken);
                return Ok(updatedSkill);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Skill with id {SkillId} not found", skillId);
                return NotFound($"Skill with id {skillId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating skill with id {SkillId}", skillId);
                return StatusCode(500, "An error occurred while updating the skill.");
            }
        }
        [HttpDelete("skills/{skillId}")]
        public async Task<IActionResult> DeleteSkill(int skillId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.DeleteSkill(skillId, cancellationToken);
                if (result)
                {
                    return Ok("Skill deleted successfully.");
                }
                return BadRequest("Failed to delete skill. Please check if the skill exists.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Skill with id {SkillId} not found", skillId);
                return NotFound($"Skill with id {skillId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting skill with id {SkillId}", skillId);
                return StatusCode(500, "An error occurred while deleting the skill.");
            }
        }
        [HttpGet("skills/{skillId}")]
        public async Task<IActionResult> GetSkillById(int skillId, CancellationToken cancellationToken)
        {
            try
            {
                var skill = await _adminService.GetSkillByIdAsync(skillId, cancellationToken);
                return Ok(skill);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Skill with id {SkillId} not found", skillId);
                return NotFound($"Skill with id {skillId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skill with id {SkillId}", skillId);
                return StatusCode(500, "An error occurred while retrieving the skill.");
            }
        }
        [HttpGet("interests")]
        public async Task<IActionResult> GetAllInterests(CancellationToken cancellationToken)
        {
            try
            {
                var interests = await _adminService.GetAllInterestsAsync(cancellationToken);
                return Ok(interests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interests.");
                return StatusCode(500, "An error occurred while retrieving interests.");
            }
        }
        [HttpGet("interests/{interestId}")]
        public async Task<IActionResult> GetInterestById(int interestId, CancellationToken cancellationToken)
        {
            try
            {
                var interest = await _adminService.GetInterestByIdAsync(interestId, cancellationToken);
                return Ok(interest);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Interest with id {InterestId} not found", interestId);
                return NotFound($"Interest with id {interestId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving interest with id {InterestId}", interestId);
                return StatusCode(500, "An error occurred while retrieving the interest.");
            }
        }
        [HttpPost("interests")]
        public async Task<IActionResult> CreateInterest([FromBody] AddInterestRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.CreateInterestAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetInterestById), new { interestId = request}, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating interest.");
                return StatusCode(500, "An error occurred while creating the interest.");
            }
        }
        [HttpGet("careergoals")]
        public async Task<IActionResult> GetAllCareerGoals(CancellationToken cancellationToken)
        {
            try
            {
                var careerGoals = await _adminService.GetAllCareerGoalsAsync(cancellationToken);
                return Ok(careerGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving career goals.");
                return StatusCode(500, "An error occurred while retrieving career goals.");
            }
        }
        [HttpPost("careergoal")]
        public async Task<IActionResult> CreateCareerGoal([FromBody] AddCareerGoalRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.CreateCareerGoalAsync(request, cancellationToken);
                return Ok("Career goal created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating career goal.");
                return StatusCode(500, "An error occurred while creating the career goal.");
            }
        }
        [HttpGet("careergoals/{careerGoalId}")]
        public async Task<IActionResult> GetCareerGoalById(int careerGoalId, CancellationToken cancellationToken)
        {
            try
            {
                var careerGoal = await _adminService.GetCareerGoalByIdAsync(careerGoalId, cancellationToken);
                return Ok(careerGoal);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Career goal with id {CareerGoalId} not found", careerGoalId);
                return NotFound($"Career goal with id {careerGoalId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving career goal with id {CareerGoalId}", careerGoalId);
                return StatusCode(500, "An error occurred while retrieving the career goal.");
            }
        }
        [HttpGet("MasterData")]
        public async Task<IActionResult> GetMasterData(CancellationToken cancellationToken)
        {
            try
            {
                var masterData = await _adminService.GetMasterDataAsync(cancellationToken);
                return Ok(masterData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving master data.");
                return StatusCode(500, "An error occurred while retrieving master data.");
            }
        }
         [HttpPost("career-goals/assign-skill")]
         public async Task<IActionResult> AssignSkillToCareerGoal([FromBody] AssignSkillToCareerGoalRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.AssignSkillToCareerGoalAsync(request, cancellationToken);
                if (result)
                {
                    return Ok("Skill assigned to career goal successfully.");
                }
                return BadRequest("Failed to assign skill to career goal. Please check if the career goal and skill exist.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Career goal with id {CareerGoalId} or skill with id {SkillId} not found", request.CareerGoalId, request.SkillId);
                return NotFound($"Career goal with id {request.CareerGoalId} or skill with id {request.SkillId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning skill with id {SkillId} to career goal with id {CareerGoalId}", request.SkillId, request.CareerGoalId);
                return StatusCode(500, "An error occurred while assigning skill to career goal.");
            }
        }
        [HttpDelete("career-goals/{careerGoalId}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromCareerGoal(int careerGoalId, int skillId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.RemoveSkillFromCareerGoalAsync(careerGoalId, skillId, cancellationToken);
                if (result)
                {
                    return Ok("Skill removed from career goal successfully.");
                }
                return BadRequest("Failed to remove skill from career goal. Please check if the career goal and skill exist.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Career goal with id {CareerGoalId} or skill with id {SkillId} not found", careerGoalId, skillId);
                return NotFound($"Career goal with id {careerGoalId} or skill with id {skillId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing skill with id {SkillId} from career goal with id {CareerGoalId}", skillId, careerGoalId);
                return StatusCode(500, "An error occurred while removing skill from career goal.");
            }
        }
        [HttpDelete("career-goals/{careerGoalId}")]
        public async Task<IActionResult> DeleteCareerGoal(int careerGoalId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _adminService.DeleteCareerGoalAsync(careerGoalId, cancellationToken);
                if (result)
                {
                    return Ok("Career goal deleted successfully.");
                }
                return BadRequest("Failed to delete career goal. Please check if the career goal exists.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Career goal with id {CareerGoalId} not found", careerGoalId);
                return NotFound($"Career goal with id {careerGoalId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting career goal with id {CareerGoalId}", careerGoalId);
                return StatusCode(500, "An error occurred while deleting the career goal.");
            }
        }
    }
}
