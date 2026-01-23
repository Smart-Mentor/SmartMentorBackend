using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace SmartMentorApi.Extentions
{
    // Adds a Bearer JWT security scheme to the generated OpenAPI document
    public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();

            const string schemeName = "Bearer";

            // Define the bearer scheme
            document.Components.SecuritySchemes[schemeName] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
            };

            // Add global security requirement so protected endpoints show the lock icon
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference    
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeName
                        }
                    },
                    Array.Empty<string>()
                }
            });

            return Task.CompletedTask;
        }
    }
}
