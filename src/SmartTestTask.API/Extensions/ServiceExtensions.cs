using FluentValidation;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Mappings;
using SmartTestTask.Application.PipelineBehaviors;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Infrastructure.MessageBus;
using SmartTestTask.Infrastructure.Repositories;
using SmartTestTask.Infrastructure.Services;
using MediatR;

namespace SmartTestTask.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateContractCommand).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(CreateContractCommand).Assembly);

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IContractProcessingService, ContractProcessingService>();

        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var useServiceBus = configuration.GetValue<bool>("ServiceBus:Enabled");
        
        if (useServiceBus && !string.IsNullOrEmpty(configuration["ServiceBus:ConnectionString"]))
        {
            services.AddSingleton<IMessageBusService, ServiceBusMessageService>();
        }
        else
        {
            services.AddSingleton<IMessageBusService, InMemoryMessageBusService>();
        }

        // Background Service
        services.AddHostedService<ContractProcessingBackgroundService>();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins(
                            "https://localhost:3000",
                            "https://localhost:5001")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        return services;
    }
}