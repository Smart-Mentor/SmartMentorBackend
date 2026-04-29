using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AdminService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Application.Implementations.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly        RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AdminService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users.");
            var users = await _unitOfWork.Repository<ApplicationUser>().GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {UserCount} users.", users.Count());
            return users;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user with id {UserId}.", userId);
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(new object[] { userId }, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} was not found.", userId);
                throw new KeyNotFoundException($"User with id {userId} not found.");
            }

            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting user with id {UserId}.", userId);
            var user = await GetUserByIdAsync(userId, cancellationToken);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with id {UserId} was deleted successfully.", userId);
                return true;
            }

            _logger.LogWarning("Failed to delete user with id {UserId}: {Errors}.",
                userId,
                string.Join(", ", result.Errors.Select(error => error.Description)));
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving roles for user with id {UserId}.", userId);
            var user = await GetUserByIdAsync(userId, cancellationToken);

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AssignRoleToUserAsync(UserRoleRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Assigning role {RoleName} to user {UserId}.", request.RoleName, request.UserId);
            var user = await GetUserByIdAsync(Guid.Parse(request.UserId), cancellationToken);
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
            {
                _logger.LogWarning("Role {RoleName} does not exist and cannot be assigned to user {UserId}.", request.RoleName, request.UserId);
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} assigned to user {UserId} successfully.", request.RoleName, request.UserId);
            }
            else
            {
                _logger.LogWarning("Failed to assign role {RoleName} to user {UserId}: {Errors}.",
                    request.RoleName,
                    request.UserId,
                    string.Join(", ", result.Errors.Select(error => error.Description)));
            }

            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromUserAsync(UserRoleRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing role {RoleName} from user {UserId}.", request.RoleName, request.UserId);
            var user = await GetUserByIdAsync(Guid.Parse(request.UserId), cancellationToken);
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
            {
                _logger.LogWarning("Role {RoleName} does not exist and cannot be removed from user {UserId}.", request.RoleName, request.UserId);
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} removed from user {UserId} successfully.", request.RoleName, request.UserId);
            }
            else
            {
                _logger.LogWarning("Failed to remove role {RoleName} from user {UserId}: {Errors}.",
                    request.RoleName,
                    request.UserId,
                    string.Join(", ", result.Errors.Select(error => error.Description)));
            }

            return result.Succeeded;
        }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all skills.");
            var skills = await _unitOfWork.Repository<Skill>().GetAllAsync(cancellationToken);
            if (skills == null)
            {
                _logger.LogWarning("No skills were returned from the repository.");
                throw new Exception("No skills found.");
            }

            _logger.LogInformation("Retrieved {SkillCount} skills.", skills.Count());
            return skills;
        }

        public async Task<Skill> GetSkillByIdAsync(int skillId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving skill with id {SkillId}.", skillId);
            var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(new object[] { skillId }, cancellationToken);
            if (skill == null)
            {
                _logger.LogWarning("Skill with id {SkillId} was not found.", skillId);
                throw new KeyNotFoundException($"Skill with id {skillId} not found.");
            }

            return skill;
        }

        public async Task CreateSkillAsync(AddSkillRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating skill with name {SkillName}.", request.Name);
            var skill = new Skill
            {
                Name = request.Name,
                Category = request.Category
            };

            await _unitOfWork.Repository<Skill>().AddAsync(skill, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Skill with name {SkillName} created successfully.", request.Name);
        }

        public async Task<Skill> UpdateSkillAsync(int skillId, AddSkillRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating skill with id {SkillId}.", skillId);
            var gettedSkill = await _unitOfWork.Repository<Skill>().GetByIdAsync(new object[] { skillId }, cancellationToken);
            if (gettedSkill == null)
            {
                _logger.LogWarning("Skill with id {SkillId} was not found for update.", skillId);
                throw new KeyNotFoundException($"Skill with id {skillId} not found.");
            }

            gettedSkill.Name = request.Name;
            gettedSkill.Category = request.Category;

            _unitOfWork.Repository<Skill>().Update(gettedSkill);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Skill with id {SkillId} was updated successfully.", skillId);

            return gettedSkill;
        }

        public async Task<bool> DeleteSkill(int skillId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting skill with id {SkillId}.", skillId);
            var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(new object[] { skillId }, cancellationToken);
            if (skill == null)
            {
                _logger.LogWarning("Skill with id {SkillId} was not found for deletion.", skillId);
                throw new KeyNotFoundException($"Skill with id {skillId} not found.");
            }

            _unitOfWork.Repository<Skill>().Delete(skill);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Skill with id {SkillId} was deleted successfully.", skillId);

            return true;
        }

    }
}