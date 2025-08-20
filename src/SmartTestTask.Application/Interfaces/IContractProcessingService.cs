using SmartTestTask.Application.Types;

namespace SmartTestTask.Application.Interfaces;

//For background processes
public interface IContractProcessingService
{
    Task ProcessContractCreatedAsync(ContractCreatedMessage message, CancellationToken cancellationToken = default);
    Task ProcessContractUpdatedAsync(ContractUpdatedMessage message, CancellationToken cancellationToken = default);
    Task ProcessContractDeactivatedAsync(ContractDeactivatedMessage message, CancellationToken cancellationToken = default);
}