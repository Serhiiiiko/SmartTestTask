using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Exceptions;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Application.Handlers.Commands;

public class UpdateContractCommandHandler : IRequestHandler<UpdateContractCommand, ApiResponse<ContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateContractCommandHandler> _logger;

    public UpdateContractCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<ContractDto>> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdWithIncludesAsync(request.Id, cancellationToken);
            
            if (contract == null)
            {
                return ApiResponse<ContractDto>.FailureResponse($"Contract with ID '{request.Id}' not found");
            }

            // Get facility to check available area
            var facility = await _unitOfWork.ProductionFacilities
                .GetByCodeWithContractsAsync(contract.ProductionFacilityCode, cancellationToken);

            // Calculate new required area
            var currentArea = contract.GetTotalArea();
            var newArea = contract.ProcessEquipmentType.Area * request.EquipmentQuantity;
            var areaDifference = newArea - currentArea;

            if (areaDifference > 0 && facility.GetAvailableArea() < areaDifference)
            {
                throw new InsufficientAreaException(
                    facility.Code,
                    areaDifference,
                    facility.GetAvailableArea());
            }

            contract.UpdateQuantity(request.EquipmentQuantity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var contractDto = _mapper.Map<ContractDto>(contract);
            
            _logger.LogInformation("Contract {ContractId} updated successfully", contract.Id);

            return ApiResponse<ContractDto>.SuccessResponse(contractDto, "Contract updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractId}", request.Id);
            return ApiResponse<ContractDto>.FailureResponse("An error occurred while updating the contract");
        }
    }
}