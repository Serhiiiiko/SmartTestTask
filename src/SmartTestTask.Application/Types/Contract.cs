namespace SmartTestTask.Application.Types;

public class ContractCreatedMessage
{
    public Guid ContractId { get; set; }
    public string FacilityCode { get; set; }
    public string EquipmentTypeCode { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContractUpdatedMessage
{
    public Guid ContractId { get; set; }
    public int NewQuantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ContractDeactivatedMessage
{
    public Guid ContractId { get; set; }
    public DateTime DeactivatedAt { get; set; }
}
