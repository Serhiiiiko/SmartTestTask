using SmartTestTask.Domain.Events;

namespace SmartTestTask.Domain.Entities;

public class EquipmentPlacementContract
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public Guid Id { get; private set; }
    public string ProductionFacilityCode { get; private set; }
    public string ProcessEquipmentTypeCode { get; private set; }
    public int EquipmentQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string ContractNumber { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public ProductionFacility ProductionFacility { get; private set; }
    public ProcessEquipmentType ProcessEquipmentType { get; private set; }
    
    // Domain events
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected EquipmentPlacementContract() { }

    public EquipmentPlacementContract(
        string productionFacilityCode, 
        string processEquipmentTypeCode, 
        int equipmentQuantity)
    {
        if (string.IsNullOrWhiteSpace(productionFacilityCode))
            throw new ArgumentException("Production facility code cannot be empty", nameof(productionFacilityCode));
        
        if (string.IsNullOrWhiteSpace(processEquipmentTypeCode))
            throw new ArgumentException("Process equipment type code cannot be empty", nameof(processEquipmentTypeCode));
        
        if (equipmentQuantity <= 0)
            throw new ArgumentException("Equipment quantity must be greater than zero", nameof(equipmentQuantity));

        Id = Guid.NewGuid();
        ProductionFacilityCode = productionFacilityCode;
        ProcessEquipmentTypeCode = processEquipmentTypeCode;
        EquipmentQuantity = equipmentQuantity;
        CreatedAt = DateTime.UtcNow;
        ContractNumber = GenerateContractNumber();
        IsActive = true;
        
        // Raise domain event
        AddDomainEvent(new ContractCreatedEvent(Id, productionFacilityCode, processEquipmentTypeCode, equipmentQuantity));
    }
    
    public decimal GetTotalArea()
    {
        return ProcessEquipmentType?.Area * EquipmentQuantity ?? 0;
    }
    
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Equipment quantity must be greater than zero", nameof(newQuantity));
        
        if (newQuantity != EquipmentQuantity)
        {
            EquipmentQuantity = newQuantity;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ContractUpdatedEvent(Id, newQuantity));
        }
    }
    
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ContractDeactivatedEvent(Id));
        }
    }
    
    private string GenerateContractNumber()
    {
        return $"EPC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
    
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}