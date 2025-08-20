namespace SmartTestTask.Application.Interfaces;

public interface IMessageBusService
{
    Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task<T> ReceiveMessageAsync<T>(CancellationToken cancellationToken = default) where T : class;
    Task StartListeningAsync(CancellationToken cancellationToken = default);
    Task StopListeningAsync(CancellationToken cancellationToken = default);
}