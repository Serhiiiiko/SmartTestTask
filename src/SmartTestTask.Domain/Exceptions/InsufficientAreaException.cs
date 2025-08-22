namespace SmartTestTask.Domain.Exceptions;

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