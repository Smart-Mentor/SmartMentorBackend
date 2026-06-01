using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace SmartMentor.Persistence.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SmartMentorApi");
            var settingsPath = Path.Combine(basePath, "appsettings.json");
            var json = File.ReadAllText(settingsPath);
            using var document = JsonDocument.Parse(json);
            var connectionString = document.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("DefaultConnection")
                .GetString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
