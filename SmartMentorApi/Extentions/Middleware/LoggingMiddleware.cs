namespace SmartMentorApi.Extensions.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class LoggingMiddleware
    {
        
        public static WebApplication UseLoggingMiddleware(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
            if (environment.IsDevelopment())
            {
              app.Use(async (context, next) =>
                {
                    logger.LogDebug("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    var authInfo = string.IsNullOrWhiteSpace(authHeader)
                        ? "None"
                        : authHeader.Split(' ', 2)[0]; // log only the scheme (e.g., "Bearer")
                    logger.LogDebug("Authorization Header (scheme only): {AuthScheme}", authInfo);
                    await next();
                    logger.LogInformation("Response Status: {StatusCode}", context.Response.StatusCode);
                });            
             return app; 
            }
        return app;
        }

    }
}