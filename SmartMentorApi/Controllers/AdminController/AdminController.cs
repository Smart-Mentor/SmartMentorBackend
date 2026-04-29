using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AdminService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentorApi.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService,
         IUnitOfWork unitOfWork,
         ILogger<AdminController> logger
         )
        {
            _adminService = adminService;
            _logger = logger;
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
        
    }
}