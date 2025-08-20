namespace SmartTestTask.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract class DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; }
    
    protected DomainEventBase()
    {
        OccurredOn = DateTime.UtcNow;
    }
}

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

public class ContractUpdatedEvent : DomainEventBase
{
    public Guid ContractId { get; }
    public int NewQuantity { get; }
    
    public ContractUpdatedEvent(Guid contractId, int newQuantity)
    {
        ContractId = contractId;
        NewQuantity = newQuantity;
    }
}

public class ContractDeactivatedEvent : DomainEventBase
{
    public Guid ContractId { get; }
    
    public ContractDeactivatedEvent(Guid contractId)
    {
        ContractId = contractId;
    }
}