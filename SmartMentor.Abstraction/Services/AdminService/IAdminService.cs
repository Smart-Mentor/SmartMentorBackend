using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Abstraction.Dto.Responses.AdminResponse;
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
        Task<IEnumerable<SkillResponse>> GetAllSkillsAsync(CancellationToken cancellationToken = default);

        Task<SkillResponse> GetSkillByIdAsync(int skillId, CancellationToken cancellationToken = default);
        Task CreateSkillAsync(AddSkillRequest request, CancellationToken cancellationToken = default);

        Task<SkillResponse> UpdateSkillAsync(int skillId,AddSkillRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteSkill(int skillId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InterestResponse>> GetAllInterestsAsync(CancellationToken cancellationToken = default);
        Task<InterestResponse> GetInterestByIdAsync(int interestId, CancellationToken cancellationToken = default);
        Task CreateInterestAsync(AddInterestRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CareerGoalResponse>> GetAllCareerGoalsAsync(CancellationToken cancellationToken = default);
        Task<CareerGoalResponse> GetCareerGoalByIdAsync(int careerGoalId, CancellationToken cancellationToken = default);
        Task CreateCareerGoalAsync(AddCareerGoalRequest request, CancellationToken cancellationToken = default);
        Task<MasterDataResponse> GetMasterDataAsync(CancellationToken cancellationToken = default);

        Task <bool>AssignSkillToCareerGoalAsync(AssignSkillToCareerGoalRequest request, CancellationToken cancellationToken = default);
    }
}