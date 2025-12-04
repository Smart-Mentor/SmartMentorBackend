
using Serilog;
using SmartMentorApi.Extentions;

namespace SmartMentorApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container (this configures Serilog properly)
                builder.AddServices();
                var app = builder.Build();

                app.UseSerilogRequestLogging();
                app.ConfigScalar();

                app.UseHttpsRedirection();

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
