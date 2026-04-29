using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Abstraction.Services.AdminService
{
    public interface IAdminService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<ApplicationUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<bool> AssignRoleToUserAsync(UserRoleRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoveRoleFromUserAsync(UserRoleRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<Skill>> GetAllSkillsAsync(CancellationToken cancellationToken = default);

        Task<Skill> GetSkillByIdAsync(int skillId, CancellationToken cancellationToken = default);
        Task CreateSkillAsync(AddSkillRequest request, CancellationToken cancellationToken = default);

        Task<Skill> UpdateSkillAsync(int skillId,AddSkillRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteSkill(int skillId, CancellationToken cancellationToken = default);
    }
}