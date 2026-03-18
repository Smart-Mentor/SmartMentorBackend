namespace SmartMentor.Application.Implementations.GapAnalysisService
{
    using System.Data.Common;
    using System.Linq.Expressions;
    using Microsoft.Extensions.Logging;
    using SmartMentor.Abstraction.Dto.Responses.GapAnalysisResponse;
    using SmartMentor.Abstraction.Repositories;
    using SmartMentor.Abstraction.Services.GapAnalysisService;
    using SmartMentor.Domain.Entiies;
    using SmartMentor.Persistence.Identity;

    public class GapAnalysisService : IGapAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GapAnalysisService> _logger;

        public GapAnalysisService(
            IUnitOfWork unitOfWork,
            ILogger<GapAnalysisService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GapAnalysisResponse> AnalyzeGapAsync(Guid userId, CancellationToken cancellationToken )
        {
            _logger.LogInformation("Starting gap analysis for user {UserId}", userId);
            try
            {

                // Fetch user, career goal, and skills data from the database
                var user= await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync([userId], cancellationToken);
                if(user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    throw new KeyNotFoundException("User not found.");
                }
                // Validate the user profile is complete and has a career goal set
                if(user.IsProfileCompleted == false || user.CareerGoalId == null)
                {
                    _logger.LogWarning("User {UserId} has an incomplete profile or no career goal set", userId);
                    throw new InvalidOperationException("User profile is incomplete or career goal is not set.");
                }
                // get the career goal of the user
                var careerGoal = await _unitOfWork.Repository<CareerGoal>().GetByIdAsync([user.CareerGoalId], cancellationToken);
                if(careerGoal == null)
                {
                    _logger.LogWarning("Career goal {CareerGoalId} not found for user {UserId}", user.CareerGoalId, userId);
                    throw new KeyNotFoundException("Career goal not found.");
                }
                // get the required skills for the career goal based in the career goal id
                var requiredSkills = await _unitOfWork.Repository<CareerGoalRequiredSkill>().FindAsync( 
                    x => x.CareerGoalId == user.CareerGoalId.Value,
                    cancellationToken,
                    x=>x.Skill
                    );
                    
                // get the user's current skills
                var userSkills = await _unitOfWork.Repository<UserSkills>().FindAsync(x => x.UserId == userId, cancellationToken);
                var userSkillDict = userSkills.ToDictionary(s=>s.SkillId, s=>s.SkillLevel);

                var response = new GapAnalysisResponse
                {
                    MissingSkills = new List<SkillGapItem>(),
                    WeakSkills = new List<SkillGapItem>(),
                    ReadySkills = new List<SkillGapItem>()
                };
                foreach(var requiredSkill in requiredSkills)
                {
                    // Check for cancellation before processing each skill
                    cancellationToken.ThrowIfCancellationRequested();
                    var HasSkill= userSkillDict.TryGetValue(requiredSkill.SkillId, out var currentLevel) ;
                    var skillGapItem = new SkillGapItem
                    {
                        SkillId = requiredSkill.SkillId,
                        SkillName = requiredSkill.Skill.Name,
                        RequiredLevel = requiredSkill.RequiredLevel,
                        CurrentLevel = HasSkill ? currentLevel : null
                    };

                    if (!skillGapItem.CurrentLevel.HasValue)
                    {
                        response.MissingSkills.Add(skillGapItem);
                    }
                    else if (skillGapItem.CurrentLevel < skillGapItem.RequiredLevel)
                    {
                        response.WeakSkills.Add(skillGapItem);
                    }
                    else
                    {
                        response.ReadySkills.Add(skillGapItem);
                    }
                    
                }
                var totalRequiredSkills = response.ReadySkills.Count + response.MissingSkills.Count + response.WeakSkills.Count;
                var completionPercentage = (response.ReadySkills.Count / (double)totalRequiredSkills) * 100;
                // Compute the readiness level based on the completion percentage
                var readinessLevel = GetReadinessLevel(completionPercentage);
                return new GapAnalysisResponse
                {
                    CareerGoalName = careerGoal.Name,
                    MissingSkills = response.MissingSkills,
                    WeakSkills = response.WeakSkills,
                    ReadySkills = response.ReadySkills,
                    statusOfTheGapAnalysis = new StatusOfTheGapAnalysis
                    {
                        completionPercentage = completionPercentage,
                        readyCount = response.ReadySkills.Count,
                        weakcount = response.WeakSkills.Count,
                        missingcount = response.MissingSkills.Count,
                        readinessLevel = readinessLevel

                    }
                };
            }catch(DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred during gap analysis for user {UserId}", userId);
                throw new Exception("An error occurred while accessing the database. Please try again later.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during gap analysis for user {UserId}", userId);
                throw;
            }
        }
        public string GetReadinessLevel(double completionPercentage)
        {
            string readinessLevel;
                if(completionPercentage == 100)
                {
                    readinessLevel = "job Ready";
                }
                else if (completionPercentage >= 70 && completionPercentage < 90)
                {
                    readinessLevel = "Advanced";
                }
                else if (completionPercentage >= 40 && completionPercentage < 70)
                {
                    readinessLevel = "Intermediate";
                }
                else
                {
                    readinessLevel = "Beginner";
                }
            
                return readinessLevel;
        }
    }
}