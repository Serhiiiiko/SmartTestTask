using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Application.Handlers.Queries;

public class GetAllContractsQueryHandler : IRequestHandler<GetAllContractsQuery, ApiResponse<IEnumerable<ContractListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllContractsQueryHandler> _logger;

    public GetAllContractsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllContractsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ContractListDto>>> Handle(
        GetAllContractsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var contracts = await _unitOfWork.EquipmentPlacementContracts
                .GetAllWithIncludesAsync(cancellationToken);
            
            var contractDtos = _mapper.Map<IEnumerable<ContractListDto>>(contracts);
            
            _logger.LogInformation("Retrieved {Count} contracts", contracts.Count());
            
            return ApiResponse<IEnumerable<ContractListDto>>.SuccessResponse(contractDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts");
            return ApiResponse<IEnumerable<ContractListDto>>.FailureResponse(
                "An error occurred while retrieving contracts");
        }
    }
}
