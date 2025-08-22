using SmartTestTask.Domain.Results;

namespace SmartTestTask.Domain.Errors;

public static class DomainErrors
{
    public static class Contract
    {
        public static Error NotFound(Guid id) => 
            new("Contract.NotFound", $"Contract with ID '{id}' was not found");
        
        public static Error InvalidQuantity => 
            new("Contract.InvalidQuantity", "Equipment quantity must be greater than zero");
        
        public static Error AlreadyDeactivated => 
            new("Contract.AlreadyDeactivated", "Contract is already deactivated");
    }
    
    public static class Facility
    {
        public static Error NotFound(string code) => 
            new("Facility.NotFound", $"Production facility with code '{code}' was not found");
        
        public static Error InsufficientArea(string facilityCode, decimal required, decimal available) => 
            new("Facility.InsufficientArea", 
                $"Insufficient area in facility {facilityCode}. Required: {required} m², Available: {available} m²");
        
        public static Error InvalidCode => 
            new("Facility.InvalidCode", "Production facility code cannot be empty");
        
        public static Error InvalidName => 
            new("Facility.InvalidName", "Production facility name cannot be empty");
        
        public static Error InvalidArea => 
            new("Facility.InvalidArea", "Standard area must be greater than zero");
    }
    
    public static class Equipment
    {
        public static Error NotFound(string code) => 
            new("Equipment.NotFound", $"Process equipment type with code '{code}' was not found");
        
        public static Error InvalidCode => 
            new("Equipment.InvalidCode", "Equipment type code cannot be empty");
        
        public static Error InvalidName => 
            new("Equipment.InvalidName", "Equipment type name cannot be empty");
        
        public static Error InvalidArea => 
            new("Equipment.InvalidArea", "Area must be greater than zero");
    }
    
    public static class Validation
    {
        public static Error InvalidInput(string field) => 
            new("Validation.InvalidInput", $"Invalid input for field: {field}");
        
        public static Error RequiredField(string field) => 
            new("Validation.RequiredField", $"Field '{field}' is required");
        
        public static Error MaxLength(string field, int maxLength) => 
            new("Validation.MaxLength", $"Field '{field}' must not exceed {maxLength} characters");
        
        public static Error InvalidFormat(string field, string format) => 
            new("Validation.InvalidFormat", $"Field '{field}' must match format: {format}");
    }
    
    public static class General
    {
        public static Error UnexpectedError => 
            new("General.UnexpectedError", "An unexpected error occurred");
        
        public static Error DatabaseError => 
            new("General.DatabaseError", "A database error occurred");
        
        public static Error ConcurrencyConflict => 
            new("General.ConcurrencyConflict", "The record was modified by another user");
    }
}