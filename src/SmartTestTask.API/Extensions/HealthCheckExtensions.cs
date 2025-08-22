using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartTestTask.Infrastructure.Data;

namespace SmartTestTask.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready" });

        return services;
    }

    public static IApplicationBuilder UseHealthChecksConfiguration(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false // Liveness check - no dependencies
        });
        
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready") // Readiness check - include database
        });

        return app;
    }
}