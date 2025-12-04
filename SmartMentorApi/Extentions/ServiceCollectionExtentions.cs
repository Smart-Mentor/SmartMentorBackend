using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using SmartMentor.Persistence.Data;

namespace SmartMentorApi.Extentions
{
        public static class ServiceCollectionExtentions
        {
            public static IServiceCollection AddOpenApidocumentation(this IServiceCollection services)
            {
                services.AddOpenApi(options =>
                {
                    options.AddDocumentTransformer((doument, context, _) =>
                    {
                        doument.Info = new OpenApiInfo
                        {
                            Title = "SmartMentorMvp",
                            Description = """
                         Comprehensive API for SmartMentor platform.
                         Supports JSON responses.
                         JWT authentication required for protected endpoints.
                        """,

                            Version = "V1",
                            Contact = new OpenApiContact
                            {
                                Name = "SmartMentor",
                                Email = "SmartMentor@gmail.com"

                            }
                        };
                        return Task.CompletedTask;
                    });
                });
                return services;
            }
            public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionstring)
            {
                if (string.IsNullOrEmpty(connectionstring))
                {
                    throw new InvalidOperationException("Connection string 'ConnectionStrings' not found or is null/empty.");
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionstring);
                });
                return services;

            }
            public static IHostBuilder AddSerilog(this IHostBuilder host)
            {
                host.UseSerilog((context, configuration) =>
                {
                    configuration
                    .ReadFrom.Configuration(context.Configuration);

                });
                return host;
            }
            public static void ConfigScalar(this WebApplication app)
            {
            Log.Information(" ConfigScalar started successfully!");

            if (app.Environment.IsDevelopment())
                {
                    app.UseHsts();
                    app.MapOpenApi();
                    app.UseDeveloperExceptionPage();
                    app.MapScalarApiReference();
                }
            }
        }
    }

