using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Requests.UserRequests;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Abstraction.Services.CompleteUserProfileService.cs;
using SmartMentor.Domain.Entiies;
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
                    var skillIds = request.Skills.Select(s => s.SkillId).ToList();
                var interestIds = request.InterestIds;
                var existingSkillIds = await _unitOfWork.Repository<Skill>().FindAsync(s => skillIds.Contains(s.Id), cancellationToken);
                var existingInterestIds = await _unitOfWork.Repository<Interests>().FindAsync(i => interestIds.Contains(i.Id), cancellationToken);
                if (existingInterestIds.Count() != interestIds.Count)
                {
                    var invalidInterestIds = interestIds.Except(existingInterestIds.Select(i => i.Id));
                    _logger.LogError($"The following interest ids are invalid: {string.Join(", ", invalidInterestIds)}");
                    return Result.Fail($"The following interest ids are invalid: {string.Join(", ", invalidInterestIds)}");
                }
                if (existingSkillIds.Count() != skillIds.Count)
                {
                    var invalidSkillIds = skillIds.Except(existingSkillIds.Select(s => s.Id));
                    _logger.LogError($"The following skill ids are invalid: {string.Join(", ", invalidSkillIds)}");
                    return Result.Fail($"The following skill ids are invalid: {string.Join(", ", invalidSkillIds)}");
                }

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
            catch
            {
                _logger.LogError($"An error occurred while completing the user profile with Id {userId}.");
                return Result.Fail($"An error occurred while completing the user profile with Id {userId}.");
            }
            
        }
    }
}
