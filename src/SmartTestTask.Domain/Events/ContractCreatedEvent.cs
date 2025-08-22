namespace SmartTestTask.Domain.Events;

public class ContractCreatedEvent : DomainEventBase
{
    public Guid ContractId { get; }
    public string FacilityCode { get; }
    public string EquipmentTypeCode { get; }
    public int Quantity { get; }
    
    public ContractCreatedEvent(Guid contractId, string facilityCode, string equipmentTypeCode, int quantity)
    {
        ContractId = contractId;
        FacilityCode = facilityCode;
        EquipmentTypeCode = equipmentTypeCode;
        Quantity = quantity;
    }
}