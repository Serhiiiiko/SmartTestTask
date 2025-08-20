namespace SmartTestTask.Domain.Entities;

public class ProcessEquipmentType
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal Area { get; private set; }
    
    // Navigation property
    public ICollection<EquipmentPlacementContract> Contracts { get; private set; }

    protected ProcessEquipmentType() 
    { 
        Contracts = new HashSet<EquipmentPlacementContract>();
    }

    public ProcessEquipmentType(string code, string name, decimal area) : this()
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Equipment type code cannot be empty", nameof(code));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment type name cannot be empty", nameof(name));
        
        if (area <= 0)
            throw new ArgumentException("Area must be greater than zero", nameof(area));

        Code = code;
        Name = name;
        Area = area;
    }
    
    public decimal CalculateTotalArea(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
        return Area * quantity;
    }
}