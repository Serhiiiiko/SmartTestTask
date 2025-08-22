namespace SmartTestTask.Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
    public string EntityType { get; }
    public string EntityId { get; }
    
    public EntityNotFoundException(string entityType, string entityId)
        : base($"{entityType} with ID '{entityId}' was not found")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}