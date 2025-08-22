using Microsoft.EntityFrameworkCore;
using SmartTestTask.Infrastructure.Data;
using Serilog;

namespace SmartTestTask.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(30);
            });
            
            // Enable detailed errors only in Development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        // Health checks

        return services;
    }

    public static async Task<IApplicationBuilder> MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                Log.Information("Applying {Count} pending migrations", pendingMigrations.Count());
                await dbContext.Database.MigrateAsync();
                Log.Information("Database migration completed successfully");
            }
            else
            {
                Log.Information("Database is up to date");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating the database");
            
            // In production, we might want to fail fast here
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                throw;
            }
        }

        return app;
    }
}