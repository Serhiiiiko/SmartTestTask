using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;
using SmartTestTask.Domain.Errors;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Handlers.Queries;

public class GetAllEquipmentTypesQueryHandler : IRequestHandler<GetAllEquipmentTypesQuery, Result<IEnumerable<ProcessEquipmentTypeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllEquipmentTypesQueryHandler> _logger;

    public GetAllEquipmentTypesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllEquipmentTypesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProcessEquipmentTypeDto>>> Handle(
        GetAllEquipmentTypesQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var equipmentTypes = await _unitOfWork.ProcessEquipmentTypes
                .GetAllAsync(cancellationToken);
            
            var equipmentTypeDtos = _mapper.Map<IEnumerable<ProcessEquipmentTypeDto>>(equipmentTypes);
            
            _logger.LogInformation("Retrieved {Count} equipment types", equipmentTypes.Count());
            
            return Result.Success(equipmentTypeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving equipment types");
            return DomainErrors.General.UnexpectedError;
        }
    }
}