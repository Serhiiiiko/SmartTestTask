using MediatR;
using SmartTestTask.Application.DTOs.Responce;

namespace SmartTestTask.Application.Queries;

// Get all contracts query
public class GetAllContractsQuery : IRequest<ApiResponse<IEnumerable<ContractListDto>>>
{
}

// Get contract by ID query
public class GetContractByIdQuery : IRequest<ApiResponse<ContractDto>>
{
    public Guid Id { get; set; }
    
    public GetContractByIdQuery(Guid id)
    {
        Id = id;
    }
}

// Get contracts by facility query
public class GetContractsByFacilityQuery : IRequest<ApiResponse<IEnumerable<ContractDto>>>
{
    public string FacilityCode { get; set; }
    
    public GetContractsByFacilityQuery(string facilityCode)
    {
        FacilityCode = facilityCode;
    }
}

// Get facilities query
public class GetAllFacilitiesQuery : IRequest<ApiResponse<IEnumerable<ProductionFacilityDto>>>
{
}

// Get equipment types query
public class GetAllEquipmentTypesQuery : IRequest<ApiResponse<IEnumerable<ProcessEquipmentTypeDto>>>
{
}