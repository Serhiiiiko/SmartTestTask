using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;

namespace SmartTestTask.Infrastructure.MessageBus;

public class ServiceBusMessageService : IMessageBusService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceBusMessageService> _logger;
    private QueueClient _queueClient;
    private readonly string _connectionString;
    private readonly string _queueName;

    public ServiceBusMessageService(IConfiguration configuration, ILogger<ServiceBusMessageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration["ServiceBus:ConnectionString"];
        _queueName = _configuration["ServiceBus:QueueName"] ?? "contract-processing-queue";
        
        InitializeQueueClient();
    }

    private void InitializeQueueClient()
    {
        if (!string.IsNullOrEmpty(_connectionString))
        {
            _queueClient = new QueueClient(_connectionString, _queueName);
            _logger.LogInformation("Service Bus queue client initialized for queue: {QueueName}", _queueName);
        }
        else
        {
            _logger.LogWarning("Service Bus connection string not configured. Messages will not be sent to Service Bus.");
        }
    }

    public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        if (_queueClient == null)
        {
            _logger.LogWarning("Service Bus not configured. Skipping message send.");
            return;
        }

        try
        {
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                ContentType = "application/json",
                Label = typeof(T).Name,
                MessageId = Guid.NewGuid().ToString()
            };

            await _queueClient.SendAsync(serviceBusMessage);
            
            _logger.LogInformation("Message of type {MessageType} sent to Service Bus queue", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to Service Bus");
            throw;
        }
    }

    public async Task<T> ReceiveMessageAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        // This method is not used in the current implementation
        // Messages are processed by the background service
        throw new NotImplementedException("Use StartListeningAsync for message processing");
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        if (_queueClient == null)
        {
            _logger.LogWarning("Service Bus not configured. Cannot start listening.");
            return;
        }

        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        
        _logger.LogInformation("Started listening to Service Bus queue: {QueueName}", _queueName);
        
        await Task.CompletedTask;
    }

    public async Task StopListeningAsync(CancellationToken cancellationToken = default)
    {
        if (_queueClient != null && !_queueClient.IsClosedOrClosing)
        {
            await _queueClient.CloseAsync();
            _logger.LogInformation("Stopped listening to Service Bus queue");
        }
    }

    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        try
        {
            var body = Encoding.UTF8.GetString(message.Body);
            _logger.LogInformation("Received message: {MessageId} with label: {Label}", message.MessageId, message.Label);
            
            // Process the message based on its label
            switch (message.Label)
            {
                case nameof(ContractCreatedMessage):
                    var contractCreated = JsonSerializer.Deserialize<ContractCreatedMessage>(body);
                    // Process contract created message
                    _logger.LogInformation("Processing ContractCreatedMessage for contract: {ContractId}", contractCreated.ContractId);
                    break;
                    
                case nameof(ContractUpdatedMessage):
                    var contractUpdated = JsonSerializer.Deserialize<ContractUpdatedMessage>(body);
                    // Process contract updated message
                    _logger.LogInformation("Processing ContractUpdatedMessage for contract: {ContractId}", contractUpdated.ContractId);
                    break;
                    
                case nameof(ContractDeactivatedMessage):
                    var contractDeactivated = JsonSerializer.Deserialize<ContractDeactivatedMessage>(body);
                    // Process contract deactivated message
                    _logger.LogInformation("Processing ContractDeactivatedMessage for contract: {ContractId}", contractDeactivated.ContractId);
                    break;
                    
                default:
                    _logger.LogWarning("Unknown message label: {Label}", message.Label);
                    break;
            }

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {MessageId}", message.MessageId);
            await _queueClient.AbandonAsync(message.SystemProperties.LockToken);
        }
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        _logger.LogError(exceptionReceivedEventArgs.Exception, 
            "Message handler encountered an exception. Context: {ExceptionContext}", 
            exceptionReceivedEventArgs.ExceptionReceivedContext);
        
        return Task.CompletedTask;
    }
}

// In-Memory fallback implementation for development
public class InMemoryMessageBusService : IMessageBusService
{
    private readonly ILogger<InMemoryMessageBusService> _logger;
    private readonly Queue<object> _messageQueue = new();

    public InMemoryMessageBusService(ILogger<InMemoryMessageBusService> logger)
    {
        _logger = logger;
    }

    public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        _messageQueue.Enqueue(message);
        _logger.LogInformation("Message of type {MessageType} added to in-memory queue. Queue size: {QueueSize}", 
            typeof(T).Name, _messageQueue.Count);
        
        await Task.CompletedTask;
    }

    public async Task<T> ReceiveMessageAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        if (_messageQueue.TryDequeue(out var message) && message is T typedMessage)
        {
            _logger.LogInformation("Message of type {MessageType} retrieved from in-memory queue", typeof(T).Name);
            return typedMessage;
        }
        
        return await Task.FromResult<T>(null);
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("In-memory message bus started listening");
        await Task.CompletedTask;
    }

    public async Task StopListeningAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("In-memory message bus stopped listening");
        _messageQueue.Clear();
        await Task.CompletedTask;
    }
}