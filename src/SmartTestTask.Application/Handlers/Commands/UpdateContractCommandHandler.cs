using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;
using SmartTestTask.Domain.Errors;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Handlers.Commands;

public class UpdateContractCommandHandler : IRequestHandler<UpdateContractCommand, Result<ContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBus;
    private readonly ILogger<UpdateContractCommandHandler> _logger;

    public UpdateContractCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMessageBusService messageBus,
        ILogger<UpdateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<ContractDto>> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdWithIncludesAsync(request.Id, cancellationToken);
            
            if (contract == null)
            {
                return DomainErrors.Contract.NotFound(request.Id);
            }

            // Get facility to check available area
            var facility = await _unitOfWork.ProductionFacilities
                .GetByCodeWithContractsAsync(contract.ProductionFacilityCode, cancellationToken);

            // Get equipment type
            var equipmentType = await _unitOfWork.ProcessEquipmentTypes
                .GetByCodeAsync(contract.ProcessEquipmentTypeCode, cancellationToken);

            // Calculate new required area
            var currentArea = contract.GetTotalArea();
            var newArea = equipmentType.Area * request.EquipmentQuantity;
            var areaDifference = newArea - currentArea;

            if (areaDifference > 0 && facility.GetAvailableArea() < areaDifference)
            {
                return DomainErrors.Facility.InsufficientArea(
                    facility.Code,
                    areaDifference,
                    facility.GetAvailableArea());
            }

            contract.UpdateQuantity(request.EquipmentQuantity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send message to Service Bus
            await _messageBus.SendMessageAsync(new ContractUpdatedMessage
            {
                ContractId = contract.Id,
                NewQuantity = request.EquipmentQuantity,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            var contractDto = _mapper.Map<ContractDto>(contract);
            
            _logger.LogInformation("Contract {ContractId} updated successfully", contract.Id);

            return Result.Success(contractDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractId}", request.Id);
            return DomainErrors.General.UnexpectedError;
        }
    }
}