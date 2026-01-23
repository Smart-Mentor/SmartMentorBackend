
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
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    var authInfo = string.IsNullOrWhiteSpace(authHeader)
                        ? "None"
                        : authHeader.Split(' ', 2)[0]; // log only the scheme (e.g., "Bearer")
                    Log.Information("Authorization Header (scheme only): {AuthScheme}", authInfo);
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
