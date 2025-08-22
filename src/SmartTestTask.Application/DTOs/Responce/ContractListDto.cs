namespace SmartTestTask.Application.DTOs.Responce;

public record ContractListDto(
    string ProductionFacilityName,
    string ProcessEquipmentTypeName,
    int EquipmentQuantity);