
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
                app.UseRouting();

                app.UseHttpsRedirection();
                app.Use(async (context, next) =>
                {
                    Log.Information("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                    Log.Information("Authorization Header: {Auth}",
                        context.Request.Headers["Authorization"].ToString());
                    await next();
                    Log.Information("Response Status: {StatusCode}", context.Response.StatusCode);
                });
                app.UseAuthentication();
                app.UseAuthorization();
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
