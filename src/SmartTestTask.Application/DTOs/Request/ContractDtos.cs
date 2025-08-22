namespace SmartTestTask.Application.DTOs.Request;

public record CreateContractDto(
    string ProductionFacilityCode,
    string ProcessEquipmentTypeCode,
    int EquipmentQuantity);

public record UpdateContractDto(
    Guid Id,
    int EquipmentQuantity);