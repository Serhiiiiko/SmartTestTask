namespace SmartTestTask.Application.DTOs.Responce;

public class ContractDto
{
    public Guid Id { get; set; }
    public required string ContractNumber { get; set; }
    public required string ProductionFacilityName { get; set; }
    public required string ProcessEquipmentTypeName { get; set; }
    public int EquipmentQuantity { get; set; }
    public decimal TotalArea { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsActive { get; set; }
}

public class ContractListDto
{
    public required string ProductionFacilityName { get; set; }
    public required string ProcessEquipmentTypeName { get; set; }
    public int EquipmentQuantity { get; set; }
}

public class ProductionFacilityDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal StandardArea { get; set; }
    public decimal OccupiedArea { get; set; }
    public decimal AvailableArea { get; set; }
}

public class ProcessEquipmentTypeDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal Area { get; set; }
}