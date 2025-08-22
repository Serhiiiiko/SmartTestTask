namespace SmartTestTask.Domain.Events;

public class ContractDeactivatedEvent : DomainEventBase
{
    public Guid ContractId { get; }
    
    public ContractDeactivatedEvent(Guid contractId)
    {
        ContractId = contractId;
    }
}