namespace SmartTestTask.Domain.Entities;

public class ProductionFacility
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal StandardArea { get; private set; }
    
    // Navigation property
    public ICollection<EquipmentPlacementContract> Contracts { get; private set; }

    protected ProductionFacility() 
    { 
        Contracts = new HashSet<EquipmentPlacementContract>();
    }

    public ProductionFacility(string code, string name, decimal standardArea) : this()
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Production facility code cannot be empty", nameof(code));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Production facility name cannot be empty", nameof(name));
        
        if (standardArea <= 0)
            throw new ArgumentException("Standard area must be greater than zero", nameof(standardArea));

        Code = code;
        Name = name;
        StandardArea = standardArea;
    }
    
    public decimal GetOccupiedArea()
    {
        return Contracts?.Sum(c => c.GetTotalArea()) ?? 0;
    }
    
    public decimal GetAvailableArea()
    {
        return StandardArea - GetOccupiedArea();
    }
    
    public bool CanAccommodateEquipment(ProcessEquipmentType equipmentType, int quantity)
    {
        if (equipmentType == null || quantity <= 0)
            return false;
            
        var requiredArea = equipmentType.Area * quantity;
        return GetAvailableArea() >= requiredArea;
    }
}