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
            var env = builder.Environment;
            host.AddSerilog();
            
            // Bind JwtSettings to configuration
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            services.AddOpenApidocumentation();
            services.AddControllers();
            // services.AddAuthorization();
            // services.AddHttpContextAccessor();
            services.AddDatabase(configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("The Connectionstring is Null or Empty "));
            services.AddIdenttiyExtention();
            services.ConfigureJwt(configuration,env);
            services.AddTransient<DataSeeder>();
            services.AddTransient<UserSkillsInterestsSeeder>();
            services.RegisterServices();
            
            return builder;
        }

    }
}
