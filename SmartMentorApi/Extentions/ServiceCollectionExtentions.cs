using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using SmartMentor.Abstraction.Services.AuthenticationService;
using SmartMentor.Application.Implementations.AuthenticationService;
using SmartMentor.Persistence.Data;
using SmartMentor.Persistence.Identity;
using System.Text;

namespace SmartMentorApi.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Register application services here
            services.AddScoped<IAuthService , AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            
            return services;
        }
        public static IServiceCollection AddOpenApidocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
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
                options.SignIn.RequireConfirmedEmail = false;
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
        public static IServiceCollection ConfigureJwt(this IServiceCollection services,IConfiguration configuration,IWebHostEnvironment env)
        {

            var jwtKey = configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                Log.Logger.Debug("JWT secret key not found in configuration. ");
                throw new InvalidOperationException("JWT secret key not found in configuration. ");
            }
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = configuration["JwtSettings:Audience"],
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                    if (env.IsDevelopment())
                    {
                        // Add these event handlers for debugging
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Log.Error("Authentication failed: {Message}", context.Exception.Message);
                                Log.Error("Exception: {Exception}", context.Exception.ToString());
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Log.Debug("Token validated successfully");
                                var claims = context.Principal.Claims.Select(c => $"{c.Type}={c.Value}");
                                Log.Information("Claims: {Claims}", string.Join(", ", claims));
                                return Task.CompletedTask;
                            },
                            OnChallenge = context =>
                            {
                                Log.Debug("OnChallenge: {Error}, {ErrorDescription}", 
                                    context.Error, context.ErrorDescription);
                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                var token = context.Token;
                                Log.Information("Token received: {TokenPreview}", 
                                    token?.Substring(0, Math.Min(20, token.Length )));
                                return Task.CompletedTask;
                            }
                        };
                    }

   
                });
                
                
            return services;
        }

    }

}

    

