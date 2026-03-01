using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Domain.Entiies;
using SmartMentor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentor.Persistence.Data
{
    public class CareerGoalRequiredSkillSeeder
    {
       private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CareerGoalRequiredSkillSeeder> _logger;

        public CareerGoalRequiredSkillSeeder(
            IUnitOfWork unitOfWork,
            ILogger<CareerGoalRequiredSkillSeeder> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                bool hasCareerGoalRequiredSkills = await _unitOfWork.Repository<CareerGoalRequiredSkill>().AnyAsync(cancellationToken: default);
                if (hasCareerGoalRequiredSkills)
                {
                    _logger.LogInformation("CareerGoalRequiredSkills already seeded.");
                    return;
                }

                _logger.LogInformation("Seeding CareerGoalRequiredSkills...");

                var data = new List<CareerGoalRequiredSkill>
                {
                    new CareerGoalRequiredSkill
                    {
                        CareerGoalId = 14,
                        SkillId = 1,
                        RequiredLevel = SkillLevelEnum.Advanced,
                        Priority = 1
                    },
                    new CareerGoalRequiredSkill
                    {
                        CareerGoalId = 14,
                        SkillId = 6,
                        RequiredLevel = SkillLevelEnum.Intermediate,
                        Priority = 2
                    },
                    new CareerGoalRequiredSkill
                    {
                        CareerGoalId = 14,
                        SkillId = 11,
                        RequiredLevel = SkillLevelEnum.Beginner,
                        Priority = 3
                    },
                    new CareerGoalRequiredSkill
                    {
                        CareerGoalId = 14,
                        SkillId = 10,
                        RequiredLevel = SkillLevelEnum.Beginner,
                        Priority = 4
                    }
                };

                await _unitOfWork.Repository<CareerGoalRequiredSkill>().AddRangeAsync(data);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("CareerGoalRequiredSkills seeded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding CareerGoalRequiredSkills.");
                throw;
            }
        }
    }
}
