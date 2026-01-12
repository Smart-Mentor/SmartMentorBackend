using SmartMentor.Domain.Entiies;
using SmartMentor.Persistence.Data;
using SmartMentorApi.Extentions;

namespace SmartMentorApi
{
    public static class DiContainer
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {

            var services = builder.Services;
            var configuration = builder.Configuration;
            var host = builder.Host;
            host.AddSerilog();
            
            // Bind JwtSettings to configuration
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            services.AddOpenApidocumentation();
            services.AddControllers();
            services.AddDatabase(configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("The Connectionstring is Null or Empty "));
            services.AddIdenttiyExtention();
            services.AddTransient<DataSeeder>();
            services.ConfigureJwt(configuration);
            services.RegisterServices();
            
            return builder;
        }

    }
}
