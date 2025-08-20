using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Domain.Exceptions;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Application.Handlers.Commands;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, ApiResponse<ContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBus;
    private readonly ILogger<CreateContractCommandHandler> _logger;

    public CreateContractCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMessageBusService messageBus,
        ILogger<CreateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<ApiResponse<ContractDto>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get facility with contracts to check available area
            var facility = await _unitOfWork.ProductionFacilities
                .GetByCodeWithContractsAsync(request.ProductionFacilityCode, cancellationToken);
            
            if (facility == null)
            {
                return ApiResponse<ContractDto>.FailureResponse(
                    $"Production facility with code '{request.ProductionFacilityCode}' not found");
            }

            // Get equipment type
            var equipmentType = await _unitOfWork.ProcessEquipmentTypes
                .GetByCodeAsync(request.ProcessEquipmentTypeCode, cancellationToken);
            
            if (equipmentType == null)
            {
                return ApiResponse<ContractDto>.FailureResponse(
                    $"Process equipment type with code '{request.ProcessEquipmentTypeCode}' not found");
            }

            // Validate available area
            if (!facility.CanAccommodateEquipment(equipmentType, request.EquipmentQuantity))
            {
                var requiredArea = equipmentType.Area * request.EquipmentQuantity;
                var availableArea = facility.GetAvailableArea();
                
                throw new InsufficientAreaException(
                    facility.Code, 
                    requiredArea, 
                    availableArea);
            }

            // Create contract
            var contract = new EquipmentPlacementContract(
                request.ProductionFacilityCode,
                request.ProcessEquipmentTypeCode,
                request.EquipmentQuantity);

            await _unitOfWork.EquipmentPlacementContracts.AddAsync(contract, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get the created contract with includes for mapping
            var createdContract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdWithIncludesAsync(contract.Id, cancellationToken);

            var contractDto = _mapper.Map<ContractDto>(createdContract);

            // Send message to Service Bus for background processing
            await _messageBus.SendMessageAsync(new ContractCreatedMessage
            {
                ContractId = contract.Id,
                FacilityCode = request.ProductionFacilityCode,
                EquipmentTypeCode = request.ProcessEquipmentTypeCode,
                Quantity = request.EquipmentQuantity,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            _logger.LogInformation("Contract {ContractId} created successfully", contract.Id);

            return ApiResponse<ContractDto>.SuccessResponse(contractDto, "Contract created successfully");
        }
        catch (InsufficientAreaException ex)
        {
            _logger.LogWarning(ex, "Insufficient area for contract creation");
            return ApiResponse<ContractDto>.FailureResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return ApiResponse<ContractDto>.FailureResponse("An error occurred while creating the contract");
        }
    }
}
