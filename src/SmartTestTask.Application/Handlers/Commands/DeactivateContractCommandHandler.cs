using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Application.Handlers.Commands;

public class DeactivateContractCommandHandler : IRequestHandler<DeactivateContractCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateContractCommandHandler> _logger;

    public DeactivateContractCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeactivateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(DeactivateContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdAsync(request.Id, cancellationToken);
            
            if (contract == null)
            {
                return ApiResponse<bool>.FailureResponse($"Contract with ID '{request.Id}' not found");
            }

            contract.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Contract {ContractId} deactivated successfully", contract.Id);

            return ApiResponse<bool>.SuccessResponse(true, "Contract deactivated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating contract {ContractId}", request.Id);
            return ApiResponse<bool>.FailureResponse("An error occurred while deactivating the contract");
        }
    }
}