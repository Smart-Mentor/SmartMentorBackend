using Microsoft.Extensions.Logging;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CareerGoalRequiredSkillSeeder> _logger;

        public CareerGoalRequiredSkillSeeder(
            ApplicationDbContext context,
            ILogger<CareerGoalRequiredSkillSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                if (_context.CareerGoalRequiredSkills.Any())
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

                await _context.CareerGoalRequiredSkills.AddRangeAsync(data);
                await _context.SaveChangesAsync();

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
