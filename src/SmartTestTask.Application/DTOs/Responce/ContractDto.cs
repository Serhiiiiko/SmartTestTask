namespace SmartTestTask.Application.DTOs.Responce;

public record ContractDto(
    Guid Id,
    string ContractNumber,
    string ProductionFacilityName,
    string ProcessEquipmentTypeName,
    int EquipmentQuantity,
    decimal TotalArea,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    bool IsActive);