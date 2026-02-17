using Microsoft.EntityFrameworkCore;
using SmartMentor.Persistence.Data;

namespace SmartMentorApi.Extentions
{
    public static class DatabaseExtentions
    {
        public static async Task SeedingIntialDataForRolesAndUsers(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                var db = services.GetRequiredService<ApplicationDbContext>();
                logger.LogInformation("Applying pending migrations (if any)");
                await db.Database.MigrateAsync();
                var UserSkillsAndInterestSeeder=services.GetRequiredService<UserSkillsInterestsSeeder>();
                var seeder = services.GetRequiredService<DataSeeder>();
                await seeder.SeedRolesAndUsersAsync(services);
                await UserSkillsAndInterestSeeder.SeedUserSkillsAndInterestsAsync();
                logger.LogInformation("Database seeding completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}
