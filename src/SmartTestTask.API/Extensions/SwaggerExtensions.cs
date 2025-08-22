using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartTestTask.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartTestTask API",
                Version = "v1",
                Description = "Equipment Placement Contract Management System",
                Contact = new OpenApiContact
                {
                    Name = "SmartTestTask Team",
                    Email = "support@smarttesttask.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add API Key authentication
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key authentication using X-API-Key header. Example: \"X-API-Key: {api_key}\"",
                Name = "X-API-Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Custom schema IDs to avoid conflicts
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        // Add Swagger Annotations package if you want to use EnableAnnotations
        //services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartTestTask API v1");
            options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            
            // UI customization
            options.DocumentTitle = "SmartTestTask API Documentation";
            options.DefaultModelsExpandDepth(-1); // Hide schemas by default
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            options.DisplayRequestDuration();
            options.EnableFilter();
            options.EnableTryItOutByDefault();
        });

        return app;
    }
}