namespace SmartTestTask.Domain.Events;

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