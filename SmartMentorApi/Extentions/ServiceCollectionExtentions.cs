using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using SmartMentor.Persistence.Data;
using SmartMentor.Persistence.Identity;

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
        public static IServiceCollection AddIdenttiyExtention(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false; 
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;   
        }

        }

    }

