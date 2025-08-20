using MediatR;
using SmartTestTask.Application.DTOs.Responce;

namespace SmartTestTask.Application.Commands;

public class CreateContractCommand : IRequest<ApiResponse<ContractDto>>
{
    public string ProductionFacilityCode { get; set; }
    public string ProcessEquipmentTypeCode { get; set; }
    public int EquipmentQuantity { get; set; }
    
    public CreateContractCommand(string productionFacilityCode, string processEquipmentTypeCode, int equipmentQuantity)
    {
        ProductionFacilityCode = productionFacilityCode;
        ProcessEquipmentTypeCode = processEquipmentTypeCode;
        EquipmentQuantity = equipmentQuantity;
    }
}