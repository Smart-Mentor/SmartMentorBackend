using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.AdminRequests;
using SmartMentor.Abstraction.Dto.Responses.AdminResponse;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.AdminService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Domain.Enums;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Application.Implementations.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
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

        public async Task<IEnumerable<SkillResponse>> GetAllSkillsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all skills.");
            var skills = await _unitOfWork.Repository<Skill>().GetAllAsync(cancellationToken);
            if (skills == null)
            {
                _logger.LogWarning("No skills were returned from the repository.");
                throw new Exception("No skills found.");
            }

            _logger.LogInformation("Retrieved {SkillCount} skills.", skills.Count());
            var Allskills= skills.Select(s => new SkillResponse
            {
                Id = s.Id,
                Name = s.Name,
                Category = s.Category
            });
            return Allskills;
        }

        public async Task<SkillResponse> GetSkillByIdAsync(int skillId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving skill with id {SkillId}.", skillId);
            var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(new object[] { skillId }, cancellationToken);
            if (skill == null)
            {
                _logger.LogWarning("Skill with id {SkillId} was not found.", skillId);
                throw new KeyNotFoundException($"Skill with id {skillId} not found.");
            }
            var skillResponse = new SkillResponse
            {
                Id = skill.Id,
                Name = skill.Name,
                Category = skill.Category
            };
            return skillResponse;
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

        public async Task<SkillResponse> UpdateSkillAsync(int skillId, AddSkillRequest request, CancellationToken cancellationToken = default)
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

            var skillResponse = new SkillResponse
            {
                Id = gettedSkill.Id,
                Name = gettedSkill.Name,
                Category = gettedSkill.Category
            };
            return skillResponse;
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

        public async Task<IEnumerable<InterestResponse>> GetAllInterestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var interests =await  _unitOfWork.Repository<Interests>().GetAllAsync(cancellationToken);
                if (interests == null)                {
                    _logger.LogWarning("No interests were returned from the repository.");
                    throw new Exception("No interests found.");
                }
                _logger.LogInformation("Retrieved {InterestCount} interests.", interests.Count());
                var Allintersets= interests.Select(i => new InterestResponse
                {
                    Id = i.Id,
                    Name = i.Name
                });
                return Allintersets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving interests.");
                throw;
            }
        }

        public async Task<InterestResponse> GetInterestByIdAsync(int interestId, CancellationToken cancellationToken = default)
        {
            if (interestId <= 0)
            {
                _logger.LogWarning("Invalid interest id {InterestId} provided for retrieval.", interestId);
                throw new ArgumentException("Interest id must be greater than zero.", nameof(interestId));
            }
            var interest = await _unitOfWork.Repository<Interests>().GetByIdAsync(new object[] { interestId }, cancellationToken);
            if (interest == null)
            {
                _logger.LogWarning("Interest with id {InterestId} was not found.", interestId);
                throw new KeyNotFoundException($"Interest with id {interestId} not found.");
            }

            return new InterestResponse
            {
                Id = interest.Id,
                Name = interest.Name
            };
        }

        public async Task CreateInterestAsync(AddInterestRequest request, CancellationToken cancellationToken = default)
        {
            var interest = new Interests
            {
                Name = request.Name
            };

            await _unitOfWork.Repository<Interests>().AddAsync(interest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        

        public async Task<IEnumerable<CareerGoalResponse>> GetAllCareerGoalsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var careerGoals=await _unitOfWork.Repository<CareerGoal>().GetAllAsync(cancellationToken);
                if (careerGoals == null)
                {
                    _logger.LogWarning("No career goals were returned from the repository.");
                    throw new Exception("No career goals found.");
                }
                _logger.LogInformation("Retrieved {CareerGoalCount} career goals.", careerGoals.Count());
                var AllCareerGoals = careerGoals.Select(cg => new CareerGoalResponse
                {
                    Id = cg.Id,
                    Name = cg.Name,
                    Description = cg.Description
                });
                return AllCareerGoals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving career goals.");
                throw;
            }
        }

        public async Task<CareerGoalResponse> GetCareerGoalByIdAsync(int careerGoalId, CancellationToken cancellationToken = default)
        {
            try
            {
                var careerGoal = await _unitOfWork.Repository<CareerGoal>().GetByIdAsync(new object[] { careerGoalId }, cancellationToken);
                if (careerGoal == null)                {
                    _logger.LogWarning("Career goal with id {CareerGoalId} was not found.", careerGoalId);
                    throw new KeyNotFoundException($"Career goal with id {careerGoalId} not found.");
                }
                return new CareerGoalResponse
                {
                    Id = careerGoal.Id,
                    Name = careerGoal.Name,
                    Description = careerGoal.Description
                };
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving career goal with id {CareerGoalId}.", careerGoalId);
                throw;
            }
        }

        public async Task<MasterDataResponse> GetMasterDataAsync(CancellationToken cancellationToken = default)
        {
            
            try{
                var  masterData = new MasterDataResponse
                {
                    Skills = await GetAllSkillsAsync(cancellationToken),
                    Interests = await GetAllInterestsAsync(cancellationToken),
                    CareerGoals = await GetAllCareerGoalsAsync(cancellationToken)
                };
            return masterData;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving master data.");
                throw new Exception("An error occurred while retrieving master data.");
            }
        }

        public async Task<bool> AssignSkillToCareerGoalAsync(
            AssignSkillToCareerGoalRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
              
                if (request.CareerGoalId <= 0 || request.SkillId <= 0)
                {
                    _logger.LogWarning("Invalid CareerGoalId {CareerGoalId} or SkillId {SkillId}", request.CareerGoalId, request.SkillId);
                    return false;
                }

               
                if (!Enum.IsDefined(typeof(SkillLevelEnum), request.RequiredLevel))
                {
                    return false;
                }

               
                var careerGoal = await _unitOfWork.Repository<CareerGoal>()
                    .GetByIdAsync(new object[] { request.CareerGoalId }, cancellationToken);

                var skill = await _unitOfWork.Repository<Skill>()
                    .GetByIdAsync(new object[] { request.SkillId }, cancellationToken);

                if (careerGoal == null)
                    return false;

                if (skill == null)
                    return false;

                //  Check duplicate
                var existing = await _unitOfWork.Repository<CareerGoalRequiredSkill>()
                    .FindAsync(x => x.CareerGoalId == request.CareerGoalId &&
                                    x.SkillId == request.SkillId,
                                    cancellationToken);

                var entity = existing.FirstOrDefault();

                if (entity != null)
                {
                    //  Update
                    entity.RequiredLevel = (SkillLevelEnum)request.RequiredLevel;
                    entity.Priority = request.Priority;

                    _unitOfWork.Repository<CareerGoalRequiredSkill>().Update(entity);
                }
                else
                {
                    
                    var newEntity = new CareerGoalRequiredSkill
                    {
                        CareerGoalId = request.CareerGoalId,
                        SkillId = request.SkillId,
                        RequiredLevel = (SkillLevelEnum)request.RequiredLevel, 
                        Priority = request.Priority
                    };

                    await _unitOfWork.Repository<CareerGoalRequiredSkill>()
                        .AddAsync(newEntity, cancellationToken);
                }

                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning skill to career goal");
                throw new Exception("An error occurred while assigning skill to career goal.");
            }
        }

        public Task CreateCareerGoalAsync(AddCareerGoalRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var careerGoal = new CareerGoal
                {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.Repository<CareerGoal>().AddAsync(careerGoal, cancellationToken);
                _unitOfWork.SaveChangesAsync(cancellationToken);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a career goal.");
                throw new Exception("An error occurred while creating a career goal.");
            }
        }
    }
}