using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;
using SmartTestTask.Domain.Errors;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Handlers.Commands;

public class DeactivateContractCommandHandler : IRequestHandler<DeactivateContractCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBusService _messageBus;
    private readonly ILogger<DeactivateContractCommandHandler> _logger;

    public DeactivateContractCommandHandler(
        IUnitOfWork unitOfWork,
        IMessageBusService messageBus,
        ILogger<DeactivateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result> Handle(DeactivateContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdAsync(request.Id, cancellationToken);
            
            if (contract == null)
            {
                return DomainErrors.Contract.NotFound(request.Id);
            }

            if (!contract.IsActive)
            {
                return DomainErrors.Contract.AlreadyDeactivated;
            }

            contract.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Send message to Service Bus
            await _messageBus.SendMessageAsync(new ContractDeactivatedMessage
            {
                ContractId = contract.Id,
                DeactivatedAt = DateTime.UtcNow
            }, cancellationToken);
            
            _logger.LogInformation("Contract {ContractId} deactivated successfully", contract.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating contract {ContractId}", request.Id);
            return DomainErrors.General.UnexpectedError;
        }
    }
}