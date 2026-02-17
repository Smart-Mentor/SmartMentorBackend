using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Repositories;
using SmartMentor.Domain.Entiies;

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
               // Check if there are any user skills or interests already seeded
                var existingUserSkills = await _unitOfWork.Repository<Skill>().GetAllAsync();
                var existingUserInterests = await _unitOfWork.Repository<Interests>().GetAllAsync();
                var careerGoals = await _unitOfWork.Repository<CareerGoal>().GetAllAsync();
                if (existingUserSkills.Any() || existingUserInterests.Any()||careerGoals.Any())
                {
                    _logger.LogInformation("User skills ,careergoal and interests already exist. Skipping seeding.");
                    return;
                }

                // Seed user skills and interests here (you can customize this with actual data)
                var Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Category = "Programming" },
                    new Skill { Name = "JavaScript", Category = "Programming" },
                    new Skill { Name = "Project Management", Category = "Management" },
                    new Skill { Name = "Data Analysis", Category = "Data Science" },
                    new Skill { Name = "Machine Learning", Category = "Data Science" },
                    new Skill { Name = "SqL", Category = "Database" },
                    new Skill { Name = "C++", Category = "Programming" },
                    new Skill { Name = "Html", Category = "Web Development" },
                    new Skill { Name = "Css", Category = "Web Development" },
                    new Skill { Name="Python", Category = "Programming" },   
                    new Skill {Name="Docker",Category="DevOps"}                 // Add more user skills as needed
                };

                var Interests = new List<Interests>
                {
                    new Interests { Name = "Web Development" },
                    new Interests { Name = "Data Science" },
                    new Interests { Name = "Mobile App Development" },
                    new Interests { Name = "Cloud Computing" },
                    new Interests { Name = "Cybersecurity" },
                    new Interests { Name = "Artificial Intelligence" },
                    new Interests { Name = "Game Development" },
                    new Interests { Name = "DevOps" },
                    new Interests { Name = "UI/UX Design" },
                    new Interests { Name = "Blockchain" }
                    // Add more user interests as needed
                };
                var careerGoal =new List<CareerGoal>
                {
                    new CareerGoal { Name = "Become a Full-Stack Developer",Description="Aspire to master both frontend and backend technologies to build complete web applications." },
                    new CareerGoal { Name = "Backend Developer",Description="Focus on server-side development, working with databases, APIs, and server logic to create robust applications." },
                    new CareerGoal { Name = "Frontend Developer",Description="Specialize in creating engaging user interfaces and experiences using HTML, CSS, and JavaScript frameworks." },
                    new CareerGoal { Name = "Transition to Data Science" ,Description="Aim to leverage programming skills to analyze data, build machine learning models, and derive insights for informed decision-making."},
                    new CareerGoal { Name = "Software engineer" ,Description="Aspire to design, develop, and maintain software applications across various domains, utilizing programming skills to solve complex problems."},
                    new CareerGoal { Name = "Data Analysist",Description="Focus on analyzing and interpreting data to help organizations make informed decisions, using programming skills to manipulate and visualize data effectively."},
                    new CareerGoal { Name = "Specialize in Machine Learning",Description="Aim to develop expertise in machine learning algorithms and techniques, applying programming skills to build predictive models and intelligent systems."},
                    new CareerGoal { Name = "Database Administrator",Description="Focus on managing and optimizing databases, ensuring data integrity, security, and performance for applications."},
                    new CareerGoal { Name = "Game Developer" ,Description="Aspire to create interactive and immersive gaming experiences, utilizing programming skills to design game mechanics, graphics, and user interactions."},
                    new CareerGoal { Name = "Cloud Solutions Architect",Description="Aim to design and implement cloud-based solutions, leveraging programming skills to build scalable and efficient applications in cloud environments."},
                    new CareerGoal { Name = "Cybersecurity Specialist",Description="Focus on protecting systems and data from cyber threats, using programming skills to develop security measures and respond to incidents."},
                    new CareerGoal { Name = "AI Researcher",Description="Aspire to conduct research in artificial intelligence, utilizing programming skills to develop and test new algorithms and models in the field of AI."}
                     // Add more career goals as needed;
                    // Add more career goals as needed;
                };
           
                await _unitOfWork.Repository<Skill>().AddRangeAsync(Skills,cancellationToken: default);
                
                await _unitOfWork.Repository<Interests>().AddRangeAsync(Interests,cancellationToken: default);
                
                await _unitOfWork.Repository<CareerGoal>().AddRangeAsync(careerGoal,cancellationToken: default);
               
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded user skills ,carrer goals and interests.");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding user skills and interests.");
                throw;
            }
        }
    }
}