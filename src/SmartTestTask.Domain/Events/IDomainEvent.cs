namespace SmartTestTask.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}