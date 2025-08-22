using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;
using SmartTestTask.Domain.Errors;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Handlers.Queries;

public class GetAllFacilitiesQueryHandler : IRequestHandler<GetAllFacilitiesQuery, Result<IEnumerable<ProductionFacilityDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllFacilitiesQueryHandler> _logger;

    public GetAllFacilitiesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllFacilitiesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProductionFacilityDto>>> Handle(
        GetAllFacilitiesQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var facilities = await _unitOfWork.ProductionFacilities
                .GetAllAsync(cancellationToken);
            
            var facilityDtos = new List<ProductionFacilityDto>();
            
            foreach (var facility in facilities)
            {
                var withContracts = await _unitOfWork.ProductionFacilities
                    .GetByCodeWithContractsAsync(facility.Code, cancellationToken);
                
                facilityDtos.Add(new ProductionFacilityDto(
                    Code: facility.Code,
                    Name: facility.Name,
                    StandardArea: facility.StandardArea,
                    OccupiedArea: withContracts.GetOccupiedArea(),
                    AvailableArea: withContracts.GetAvailableArea()
                ));
            }
            
            _logger.LogInformation("Retrieved {Count} facilities", facilities.Count());
            
            return Result.Success(facilityDtos.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facilities");
            return DomainErrors.General.UnexpectedError;
        }
    }
}