using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;
using SmartTestTask.Infrastructure.MessageBus;

namespace SmartTestTask.Infrastructure.Services;

public class ContractProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContractProcessingBackgroundService> _logger;
    private readonly IMessageBusService _messageBusService;

    public ContractProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ContractProcessingBackgroundService> logger,
        IMessageBusService messageBusService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _messageBusService = messageBusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Contract Processing Background Service is starting");

        await _messageBusService.StartListeningAsync(stoppingToken);

        // If using in-memory queue, poll for messages
        if (_messageBusService is InMemoryMessageBusService)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessInMemoryMessages(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background service");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }
        else
        {
            // For Service Bus, the message handler is already registered
            // Just keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

    private async Task ProcessInMemoryMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var processingService = scope.ServiceProvider.GetRequiredService<IContractProcessingService>();

        // Try to get ContractCreatedMessage
        var createdMessage = await _messageBusService.ReceiveMessageAsync<ContractCreatedMessage>(cancellationToken);
        if (createdMessage != null)
        {
            await processingService.ProcessContractCreatedAsync(createdMessage, cancellationToken);
            return;
        }

        // Try to get ContractUpdatedMessage
        var updatedMessage = await _messageBusService.ReceiveMessageAsync<ContractUpdatedMessage>(cancellationToken);
        if (updatedMessage != null)
        {
            await processingService.ProcessContractUpdatedAsync(updatedMessage, cancellationToken);
            return;
        }

        // Try to get ContractDeactivatedMessage
        var deactivatedMessage = await _messageBusService.ReceiveMessageAsync<ContractDeactivatedMessage>(cancellationToken);
        if (deactivatedMessage != null)
        {
            await processingService.ProcessContractDeactivatedAsync(deactivatedMessage, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Contract Processing Background Service is stopping");
        
        await _messageBusService.StopListeningAsync(cancellationToken);
        
        await base.StopAsync(cancellationToken);
    }
}

public class ContractProcessingService : IContractProcessingService
{
    private readonly ILogger<ContractProcessingService> _logger;

    public ContractProcessingService(ILogger<ContractProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task ProcessContractCreatedAsync(ContractCreatedMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing contract created event for Contract ID: {ContractId}", message.ContractId);
        
        // Simulate some processing work
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        
        // Here you could:
        // - Send notifications
        // - Update related systems
        // - Generate reports
        // - Update analytics
        // - Send emails
        
        _logger.LogInformation("Contract created event processed successfully for Contract ID: {ContractId}", message.ContractId);
    }

    public async Task ProcessContractUpdatedAsync(ContractUpdatedMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing contract updated event for Contract ID: {ContractId}", message.ContractId);
        
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        
        _logger.LogInformation("Contract updated event processed successfully for Contract ID: {ContractId}", message.ContractId);
    }

    public async Task ProcessContractDeactivatedAsync(ContractDeactivatedMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing contract deactivated event for Contract ID: {ContractId}", message.ContractId);
        
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        
        _logger.LogInformation("Contract deactivated event processed successfully for Contract ID: {ContractId}", message.ContractId);
    }
}