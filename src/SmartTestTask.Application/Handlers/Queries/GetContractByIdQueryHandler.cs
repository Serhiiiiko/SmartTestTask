using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Application.Handlers.Queries;

public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ApiResponse<ContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetContractByIdQueryHandler> _logger;

    public GetContractByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetContractByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<ContractDto>> Handle(
        GetContractByIdQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.EquipmentPlacementContracts
                .GetByIdWithIncludesAsync(request.Id, cancellationToken);
            
            if (contract == null)
            {
                return ApiResponse<ContractDto>.FailureResponse(
                    $"Contract with ID '{request.Id}' not found");
            }
            
            var contractDto = _mapper.Map<ContractDto>(contract);
            
            return ApiResponse<ContractDto>.SuccessResponse(contractDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contract {ContractId}", request.Id);
            return ApiResponse<ContractDto>.FailureResponse(
                "An error occurred while retrieving the contract");
        }
    }
}

public class GetContractsByFacilityQueryHandler : IRequestHandler<GetContractsByFacilityQuery, ApiResponse<IEnumerable<ContractDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetContractsByFacilityQueryHandler> _logger;

    public GetContractsByFacilityQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetContractsByFacilityQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ContractDto>>> Handle(
        GetContractsByFacilityQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var contracts = await _unitOfWork.EquipmentPlacementContracts
                .GetByFacilityCodeAsync(request.FacilityCode, cancellationToken);
            
            var contractDtos = _mapper.Map<IEnumerable<ContractDto>>(contracts);
            
            _logger.LogInformation("Retrieved {Count} contracts for facility {FacilityCode}", 
                contracts.Count(), request.FacilityCode);
            
            return ApiResponse<IEnumerable<ContractDto>>.SuccessResponse(contractDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts for facility {FacilityCode}", request.FacilityCode);
            return ApiResponse<IEnumerable<ContractDto>>.FailureResponse(
                "An error occurred while retrieving contracts");
        }
    }
}