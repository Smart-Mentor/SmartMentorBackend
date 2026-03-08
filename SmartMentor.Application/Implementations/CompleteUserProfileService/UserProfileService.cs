using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.UserRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.CompleteUserProfileService;
using SmartMentor.Domain.Entiies;
using SmartMentor.Domain.Enums;
using SmartMentor.Persistence.Identity;

namespace SmartMentor.Application.Implementations.CompleteUserProfileService
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserProfileService> _logger;
        public UserProfileService(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ILogger<UserProfileService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager=userManager;
            _logger = logger;
        }
        public async Task ValidateSkillsAndInterests(CompleteUserProfileRequest request)
        {
            var skillIds = request.Skills.Select(s => s.SkillId).ToList();
            var interestIds = request.InterestIds;

            var existingSkillIds = (await _unitOfWork.Repository<Skill>().FindAsync(s => skillIds.Contains(s.Id))).Select(s => s.Id).ToList();
            var existingInterestIds = (await _unitOfWork.Repository<Interests>().FindAsync(i => interestIds.Contains(i.Id))).Select(i => i.Id).ToList();

            if (existingInterestIds.Count() != interestIds.Count)
            {
                var invalidInterestIds = interestIds.Except(existingInterestIds);
                throw new Exception($"The following interest ids are invalid: {string.Join(", ", invalidInterestIds)}");
            }

            if (existingSkillIds.Count() != skillIds.Count)
            {
                var invalidSkillIds = skillIds.Except(existingSkillIds);
                throw new Exception($"The following skill ids are invalid: {string.Join(", ", invalidSkillIds)}");
            }
            // validate the career goal id
            var careerGoal = await _unitOfWork.Repository<CareerGoal>().GetByIdAsync([request.CareerGoalId]);
            if (careerGoal == null)
            {
                throw new Exception($"Career goal with Id {request.CareerGoalId} does not exist.");
            }
            _logger.LogInformation("Validation of skills, interests, and career goal completed successfully for career goal Id {CareerGoalId}", request.CareerGoalId);
        }
        public async Task<Result> CompleteAsync(Guid userId, CompleteUserProfileRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogDebug($"The User with Id {userId} is not found ");
                return Result.Fail($"The User with Id {userId} is not found");
            }
            try
            {
                // validate the request (check if the skill ids and interest ids are valid)
                await ValidateSkillsAndInterests(request);

                    // i want to insert the user skills and interests in the database
                var userSkills = request.Skills.Select(s => new UserSkills
                {
                    UserId = userId,
                    SkillId = s.SkillId,
                    SkillLevel = s.SkillLevel
                }).ToList();
                //
                var userInterests = request.InterestIds.Select(i => new UserInterests
                {
                    UserId = userId,
                    InterestId = i
                }).ToList();    
                // i will use the unit of work to insert the user skills and interests in the database
                await _unitOfWork.Repository<UserSkills>().AddRangeAsync(userSkills,cancellationToken);
                await _unitOfWork.Repository<UserInterests>().AddRangeAsync(userInterests,cancellationToken);
                var isCareerGoalExist = await _unitOfWork.Repository<CareerGoal>().GetByIdAsync([request.CareerGoalId],cancellationToken);
                if (isCareerGoalExist == null)
                {
                    _logger.LogError($"Career goal with Id {request.CareerGoalId} does not exist.");
                    return Result.Fail($"Career goal with Id {request.CareerGoalId} does not exist.");
                }
                // assign the CarrerGoalId FK into the user table
               user.CareerGoalId = request.CareerGoalId;
               user.IsProfileCompleted = true;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                         _logger.LogError($"Failed to update user with Id {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return Result.Fail($"Failed to update user with Id {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                _logger.LogInformation($"User profile with Id {userId} has been completed successfully.");

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while completing the user profile with Id {userId}.");
                return Result.Fail($"An error occurred while completing the user profile with Id {userId}. Error: {ex.Message}");
            }    
        }

        public async Task<Result> UpdateAsync(Guid userId, CompleteUserProfileRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating profile for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogWarning("User with Id {UserId} not found during profile update.", userId);
                return Result.Fail($"User with Id {userId} not found.");
            }

            try
            {

                // Validate the request (check if the skill ids and interest ids are valid)
                 await ValidateSkillsAndInterests(request);
               
                // Remove old skills
                var oldSkills = await _unitOfWork.Repository<UserSkills>()
                    .FindAsync(x => x.UserId == userId, cancellationToken);

                if (oldSkills.Any())
                {
                    _unitOfWork.Repository<UserSkills>().RemoveRange(oldSkills);
                    _logger.LogInformation("Removed {Count} old skills for user {UserId}", oldSkills.Count(), userId);
                }

                // Remove old interests
                var oldInterests = await _unitOfWork.Repository<UserInterests>()
                    .FindAsync(x => x.UserId == userId, cancellationToken);

                if (oldInterests.Any())
                {
                    _unitOfWork.Repository<UserInterests>().RemoveRange(oldInterests);
                    _logger.LogInformation("Removed {Count} old interests for user {UserId}", oldInterests.Count(), userId);
                }
                // Add new skills
                
                var newSkills = request.Skills.Select(s => new UserSkills
                {
                    UserId = userId,
                    SkillId = s.SkillId,
                    SkillLevel = s.SkillLevel
                }).ToList();
               
                // Add new interests
                var newInterests = request.InterestIds.Select(i => new UserInterests
                {
                    UserId = userId,
                    InterestId = i
                }).ToList();

                await _unitOfWork.Repository<UserInterests>()
                    .AddRangeAsync(newInterests, cancellationToken);

                _logger.LogInformation("Added {Count} new interests for user {UserId}", newInterests.Count, userId);

                _logger.LogInformation("Added {Count} new skills for user {UserId}", newSkills.Count, userId);
                await _unitOfWork.Repository<UserSkills>()
                    .AddRangeAsync(newSkills, cancellationToken);


                // =========================
                // Update career goal
                // =========================
                user.CareerGoalId = request.CareerGoalId;

                var identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    _logger.LogError(
                        "Failed to update identity user {UserId}. Errors: {Errors}",
                        userId,
                        string.Join(", ", identityResult.Errors.Select(e => e.Description)));

                    return Result.Fail("Failed to update user.");
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Profile updated successfully for user {UserId}", userId);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating profile for user {UserId}", userId);
                return Result.Fail("An unexpected error occurred while updating profile.");
            }
        }

        public async Task<Result<string>> updateSkillLevel(Guid userId, int skillId, CancellationToken cancellationToken = default)
        {
            // 1. Verify User
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result.Fail<string>($"User not found.");

            // 2. Get the "Target" level from the Career Goal
            var careerGoalSkill = (await _unitOfWork.Repository<CareerGoalRequiredSkill>()
                .FindAsync(cgrs => cgrs.SkillId == skillId && cgrs.CareerGoalId == user.CareerGoalId, cancellationToken))
                .FirstOrDefault();

            if (careerGoalSkill == null)
            {
                return Result.Fail<string>("This skill is not required for your current career goal.");
            }

            // 3. Check if User already possesses this skill
            var userSkill = (await _unitOfWork.Repository<UserSkills>()
                .FindAsync(us => us.UserId == userId && us.SkillId == skillId, cancellationToken))
                .FirstOrDefault();

            if (userSkill == null)
            {
                // 4. ADD logic: If they don't have it, create the link
                var newSkill = new UserSkills
                {
                    UserId = userId,
                    SkillId = skillId,
                    SkillLevel = SkillLevelEnum.Intermediate // Your default logic
                };
                await _unitOfWork.Repository<UserSkills>().AddAsync(newSkill, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(newSkill.SkillLevel.ToString());
            }

            // 5. UPDATE logic: If they have it, update to the required level
            userSkill.SkillLevel = careerGoalSkill.RequiredLevel;
            _unitOfWork.Repository<UserSkills>().Update(userSkill);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(userSkill.SkillLevel.ToString());
        }
    }
}
