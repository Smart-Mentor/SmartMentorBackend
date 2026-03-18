using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Domain.Entiies;
using SmartMentor.Domain.Enums;

namespace SmartMentor.Persistence.Data
{
    public class UserSkillsInterestsSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserSkillsInterestsSeeder> _logger;

        public UserSkillsInterestsSeeder(IUnitOfWork unitOfWork, ILogger<UserSkillsInterestsSeeder> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedUserSkillsAndInterestsAsync()
        {
            try
            {
                var existingUserSkills = await _unitOfWork.Repository<Skill>().GetAllAsync();
                var existingUserInterests = await _unitOfWork.Repository<Interests>().GetAllAsync();
                var existingCareerGoals = await _unitOfWork.Repository<CareerGoal>().GetAllAsync();

                if (existingUserSkills.Any() || existingUserInterests.Any() || existingCareerGoals.Any())
                {
                    _logger.LogInformation("Skills, interests, or career goals already exist in the database. Skipping seeding.");
                    return;
                }

                #region Skills

                var skills = new List<Skill>
                {
                    new Skill { Name = "C#", Category = "Backend Development" },
                    new Skill { Name = "ASP.NET Core Web API", Category = "Backend Development" },
                    new Skill { Name = "Entity Framework Core", Category = "Backend Development" },
                    new Skill { Name = "LINQ Queries", Category = "Backend Development" },
                    new Skill { Name = "RESTful API Design", Category = "Backend Development" },
                    new Skill { Name = "Dependency Injection", Category = "Backend Development" },

                    new Skill { Name = "HTML5", Category = "Frontend Development" },
                    new Skill { Name = "CSS3", Category = "Frontend Development" },
                    new Skill { Name = "JavaScript ES6+", Category = "Frontend Development" },
                    new Skill { Name = "React.js Fundamentals", Category = "Frontend Development" },
                    new Skill { Name = "Responsive Web Design", Category = "Frontend Development" },

                    new Skill { Name = "SQL Server", Category = "Database" },
                    new Skill { Name = "Database Normalization", Category = "Database" },
                    new Skill { Name = "Writing Complex SQL Queries", Category = "Database" },
                    new Skill { Name = "Stored Procedures", Category = "Database" },

                    new Skill { Name = "Python Programming", Category = "Data Science" },
                    new Skill { Name = "Data Analysis with Pandas", Category = "Data Science" },
                    new Skill { Name = "Data Visualization", Category = "Data Science" },
                    new Skill { Name = "Machine Learning Fundamentals", Category = "Artificial Intelligence" },

                    new Skill { Name = "Docker Containers", Category = "DevOps" },
                    new Skill { Name = "CI/CD Pipelines", Category = "DevOps" },
                    new Skill { Name = "Azure Cloud Basics", Category = "Cloud Computing" },

                    new Skill { Name = "OWASP Security Principles", Category = "Cybersecurity" },
                    new Skill { Name = "Authentication & Authorization (JWT)", Category = "Cybersecurity" }
                };

                #endregion


                #region Interests

                var interests = new List<Interests>
                {
                    new Interests { Name = "Backend Development with .NET" },
                    new Interests { Name = "Frontend Web Development" },
                    new Interests { Name = "Full-Stack Web Applications" },
                    new Interests { Name = "Data Analytics and Visualization" },
                    new Interests { Name = "Artificial Intelligence & Machine Learning" },
                    new Interests { Name = "Cloud Computing (Azure)" },
                    new Interests { Name = "DevOps & Automation" },
                    new Interests { Name = "Cybersecurity & Ethical Hacking" }
                };

                #endregion


                #region CareerGoals

                var careerGoals = new List<CareerGoal>
                {
                    new CareerGoal
                    {
                        Name = "Junior Backend .NET Developer",
                        Description = "Build and maintain RESTful APIs using ASP.NET Core and SQL Server."
                    },

                    new CareerGoal
                    {
                        Name = "Full-Stack .NET Developer",
                        Description = "Develop complete web applications using ASP.NET Core and React.js."
                    },

                    new CareerGoal
                    {
                        Name = "Frontend React Developer",
                        Description = "Create responsive and interactive user interfaces using React and modern JavaScript."
                    },

                    new CareerGoal
                    {
                        Name = "Data Analyst",
                        Description = "Analyze datasets, generate insights, and build dashboards using Python and SQL."
                    },

                    new CareerGoal
                    {
                        Name = "Machine Learning Engineer",
                        Description = "Develop predictive models and AI solutions using Python and ML frameworks."
                    },

                    new CareerGoal
                    {
                        Name = "Cloud Engineer (Azure)",
                        Description = "Design and deploy scalable applications on Microsoft Azure."
                    },

                    new CareerGoal
                    {
                        Name = "DevOps Engineer",
                        Description = "Automate deployments and manage CI/CD pipelines using Docker and cloud tools."
                    },

                    new CareerGoal
                    {
                        Name = "Cybersecurity Analyst",
                        Description = "Secure applications and infrastructure by applying modern security practices."
                    }
                };

                #endregion


                await _unitOfWork.Repository<Skill>().AddRangeAsync(skills);
                await _unitOfWork.Repository<Interests>().AddRangeAsync(interests);
                await _unitOfWork.Repository<CareerGoal>().AddRangeAsync(careerGoals);

                await _unitOfWork.SaveChangesAsync();


                #region CareerGoalSkillMappings


                var allSkills = await _unitOfWork.Repository<Skill>().GetAllAsync();
                var allGoals = await _unitOfWork.Repository<CareerGoal>().GetAllAsync();

                Skill GetSkill(string name) => allSkills.First(s => s.Name == name);
                CareerGoal GetGoal(string name) => allGoals.First(g => g.Name == name);

                var mappings = new List<CareerGoalRequiredSkill>
{
                    // Junior Backend .NET Developer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Junior Backend .NET Developer").Id, SkillId = GetSkill("C#").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Junior Backend .NET Developer").Id, SkillId = GetSkill("ASP.NET Core Web API").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Junior Backend .NET Developer").Id, SkillId = GetSkill("Entity Framework Core").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Junior Backend .NET Developer").Id, SkillId = GetSkill("SQL Server").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Junior Backend .NET Developer").Id, SkillId = GetSkill("RESTful API Design").Id, RequiredLevel = SkillLevelEnum.Beginner, Priority = 3 },

                    // Full-Stack .NET Developer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("C#").Id, RequiredLevel = SkillLevelEnum.Advanced, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("ASP.NET Core Web API").Id, RequiredLevel = SkillLevelEnum.Advanced, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("React.js Fundamentals").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("JavaScript ES6+").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("SQL Server").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("HTML5").Id, RequiredLevel = SkillLevelEnum.Beginner, Priority = 3 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Full-Stack .NET Developer").Id, SkillId = GetSkill("CSS3").Id, RequiredLevel = SkillLevelEnum.Beginner, Priority = 3 },

                    // Frontend React Developer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Frontend React Developer").Id, SkillId = GetSkill("React.js Fundamentals").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Frontend React Developer").Id, SkillId = GetSkill("JavaScript ES6+").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Frontend React Developer").Id, SkillId = GetSkill("HTML5").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Frontend React Developer").Id, SkillId = GetSkill("CSS3").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Frontend React Developer").Id, SkillId = GetSkill("Responsive Web Design").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },

                    // Data Analyst
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Data Analyst").Id, SkillId = GetSkill("Python Programming").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Data Analyst").Id, SkillId = GetSkill("Data Analysis with Pandas").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Data Analyst").Id, SkillId = GetSkill("Data Visualization").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Data Analyst").Id, SkillId = GetSkill("Writing Complex SQL Queries").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },

                    // Machine Learning Engineer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Machine Learning Engineer").Id, SkillId = GetSkill("Python Programming").Id, RequiredLevel = SkillLevelEnum.Advanced, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Machine Learning Engineer").Id, SkillId = GetSkill("Machine Learning Fundamentals").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Machine Learning Engineer").Id, SkillId = GetSkill("Data Analysis with Pandas").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },

                    // Cloud Engineer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Cloud Engineer (Azure)").Id, SkillId = GetSkill("Azure Cloud Basics").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Cloud Engineer (Azure)").Id, SkillId = GetSkill("Docker Containers").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Cloud Engineer (Azure)").Id, SkillId = GetSkill("CI/CD Pipelines").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },

                    // DevOps Engineer
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("DevOps Engineer").Id, SkillId = GetSkill("Docker Containers").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("DevOps Engineer").Id, SkillId = GetSkill("CI/CD Pipelines").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("DevOps Engineer").Id, SkillId = GetSkill("Azure Cloud Basics").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },

                    // Cybersecurity Analyst
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Cybersecurity Analyst").Id, SkillId = GetSkill("OWASP Security Principles").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 1 },
                    new CareerGoalRequiredSkill { CareerGoalId = GetGoal("Cybersecurity Analyst").Id, SkillId = GetSkill("Authentication & Authorization (JWT)").Id, RequiredLevel = SkillLevelEnum.Intermediate, Priority = 2 },
                };

                #endregion


                await _unitOfWork.Repository<CareerGoalRequiredSkill>()
                    .AddRangeAsync(mappings);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded skills, interests, career goals, and goal-skill mappings.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding user skills and interests.");
                throw;
            }
        }
    }
}