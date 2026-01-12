
using Serilog;
using SmartMentor.Persistence.Identity;
using SmartMentorApi.Extentions;
using System.Threading.Tasks;

namespace SmartMentorApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container (this configures Serilog properly)
                builder.AddServices();
                
                var app = builder.Build();

                await app.SeedingIntialDataForRolesAndUsers();
                app.UseSerilogRequestLogging();
                app.ConfigScalar();
                
                app.UseForwardedHeaders();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseRouting();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.Information("Shutting down SmartMentor Web API");
                Log.CloseAndFlush();
            }
        }
    }
}
