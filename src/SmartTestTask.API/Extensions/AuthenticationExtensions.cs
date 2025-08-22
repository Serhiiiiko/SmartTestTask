using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SmartTestTask.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("ApiKey")
            .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);
        
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions 
{
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Skip authentication for health endpoints
        if (Context.Request.Path.StartsWithSegments("/health"))
        {
            var healthClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "HealthCheck"),
                new Claim(ClaimTypes.NameIdentifier, "health")
            };
            var healthIdentity = new ClaimsIdentity(healthClaims, Scheme.Name);
            var healthPrincipal = new ClaimsPrincipal(healthIdentity);
            var healthTicket = new AuthenticationTicket(healthPrincipal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(healthTicket));
        }

        // Skip for Swagger in Development
        if (Context.Request.Path.StartsWithSegments("/swagger") && 
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var swaggerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "SwaggerUser"),
                new Claim(ClaimTypes.NameIdentifier, "swagger")
            };
            var swaggerIdentity = new ClaimsIdentity(swaggerClaims, Scheme.Name);
            var swaggerPrincipal = new ClaimsPrincipal(swaggerIdentity);
            var swaggerTicket = new AuthenticationTicket(swaggerPrincipal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(swaggerTicket));
        }

        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key was not provided"));
        }

        var apiKey = _configuration["ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Logger.LogError("API Key is not configured");
            return Task.FromResult(AuthenticateResult.Fail("API Key is not configured"));
        }

        if (!apiKey.Equals(providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "ApiKeyUser"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("ApiKey", "Valid")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}