namespace SmartTestTask.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class InsufficientAreaException : DomainException
{
    public string FacilityCode { get; }
    public decimal RequiredArea { get; }
    public decimal AvailableArea { get; }
    
    public InsufficientAreaException(string facilityCode, decimal requiredArea, decimal availableArea)
        : base($"Insufficient area in facility {facilityCode}. Required: {requiredArea} m², Available: {availableArea} m²")
    {
        FacilityCode = facilityCode;
        RequiredArea = requiredArea;
        AvailableArea = availableArea;
    }
}

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

public class BusinessRuleValidationException : DomainException
{
    public string RuleName { get; }
    
    public BusinessRuleValidationException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
    }
}

public class ContractValidationException : DomainException
{
    public ContractValidationException(string message) : base(message) { }
}