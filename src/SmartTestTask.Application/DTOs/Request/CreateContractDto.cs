namespace SmartTestTask.Application.DTOs.Request;

public class CreateContractDto
{
    public required string ProductionFacilityCode { get; set; } 
    public required string ProcessEquipmentTypeCode { get; set; }
    public int EquipmentQuantity { get; set; }
}