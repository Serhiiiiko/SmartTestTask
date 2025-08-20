using AutoMapper;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Request;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Contract mappings
        CreateMap<EquipmentPlacementContract, ContractDto>()
            .ForMember(dest => dest.ProductionFacilityName, 
                opt => opt.MapFrom(src => src.ProductionFacility != null ? src.ProductionFacility.Name : string.Empty))
            .ForMember(dest => dest.ProcessEquipmentTypeName, 
                opt => opt.MapFrom(src => src.ProcessEquipmentType != null ? src.ProcessEquipmentType.Name : string.Empty))
            .ForMember(dest => dest.TotalArea, 
                opt => opt.MapFrom(src => src.GetTotalArea()));

        CreateMap<EquipmentPlacementContract, ContractListDto>()
            .ForMember(dest => dest.ProductionFacilityName, 
                opt => opt.MapFrom(src => src.ProductionFacility != null ? src.ProductionFacility.Name : string.Empty))
            .ForMember(dest => dest.ProcessEquipmentTypeName, 
                opt => opt.MapFrom(src => src.ProcessEquipmentType != null ? src.ProcessEquipmentType.Name : string.Empty));

        // Facility mappings
        CreateMap<ProductionFacility, ProductionFacilityDto>()
            .ForMember(dest => dest.OccupiedArea, 
                opt => opt.MapFrom(src => src.GetOccupiedArea()))
            .ForMember(dest => dest.AvailableArea, 
                opt => opt.MapFrom(src => src.GetAvailableArea()));

        // Equipment type mappings
        CreateMap<ProcessEquipmentType, ProcessEquipmentTypeDto>();
        
        // DTO to Command mappings
        CreateMap<CreateContractDto, CreateContractCommand>();
        CreateMap<UpdateContractDto, UpdateContractCommand>();
    }
}