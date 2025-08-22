using Serilog;
using SmartTestTask.API.Extensions;
using SmartTestTask.API.Middleware;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/smarttesttask-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting SmartTestTask API");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Configure Serilog
    builder.Host.UseSerilog();
    
    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddMessageBus(builder.Configuration);
    builder.Services.AddApiKeyAuthentication();
    builder.Services.AddSwaggerDocumentation();
    builder.Services.AddCorsPolicy();
    builder.Services.AddHealthChecksConfiguration();
    
    // Build the application
    var app = builder.Build();
    
    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        await app.MigrateDatabaseAsync(); // Only migrate in development
    }
    else
    {
        app.UseHsts();
    }
    
    // Middleware pipeline
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    
    app.UseHttpsRedirection();
    app.UseRouting();
    
    app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowSpecificOrigins");
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseSwaggerDocumentation();
    
    app.MapControllers();
    
    // Health checks
    app.UseHealthChecksConfiguration();
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}