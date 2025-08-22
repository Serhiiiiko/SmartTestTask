namespace SmartTestTask.Application.Constants;

public static class MessageConstants
{
    public static class Contract
    {
        // Error Messages
        public static string NotFound(Guid id) => $"Contract with ID '{id}' was not found";
        public static string InvalidQuantity => "Equipment quantity must be greater than zero";
        public static string AlreadyDeactivated => "Contract is already deactivated";
        public static string CreationFailed => "Failed to create contract";
        public static string UpdateFailed => "Failed to update contract";
        public static string DeactivationFailed => "Failed to deactivate contract";
        public static string IdMismatch => "Contract ID mismatch";
        
        // Success Messages
        public static string CreatedSuccessfully => "Contract created successfully";
        public static string UpdatedSuccessfully => "Contract updated successfully";
        public static string DeactivatedSuccessfully => "Contract deactivated successfully";
        public static string RetrievedSuccessfully => "Contract retrieved successfully";
        public static string ListRetrievedSuccessfully => "Contracts retrieved successfully";
        
        // Validation Messages
        public static string FacilityCodeRequired => "Production facility code is required";
        public static string FacilityCodeMaxLength => "Production facility code must not exceed 50 characters";
        public static string FacilityCodeFormat => "Production facility code must contain only uppercase letters, numbers, and hyphens";
        public static string EquipmentCodeRequired => "Process equipment type code is required";
        public static string EquipmentCodeMaxLength => "Process equipment type code must not exceed 50 characters";
        public static string EquipmentCodeFormat => "Process equipment type code must contain only uppercase letters, numbers, and hyphens";
        public static string QuantityRequired => "Equipment quantity is required";
        public static string QuantityMustBePositive => "Equipment quantity must be greater than zero";
        public static string QuantityMaximum => "Equipment quantity must not exceed 1000";
        public static string IdRequired => "Contract ID is required";
    }
    
    public static class Facility
    {
        // Error Messages
        public static string NotFound(string code) => $"Production facility with code '{code}' was not found";
        public static string InsufficientArea(string code, decimal required, decimal available) =>
            $"Insufficient area in facility {code}. Required: {required} m², Available: {available} m²";
        public static string InvalidCode => "Production facility code cannot be empty";
        public static string InvalidName => "Production facility name cannot be empty";
        public static string InvalidArea => "Standard area must be greater than zero";
        
        // Success Messages
        public static string CreatedSuccessfully => "Facility created successfully";
        public static string UpdatedSuccessfully => "Facility updated successfully";
        public static string DeletedSuccessfully => "Facility deleted successfully";
        public static string ListRetrievedSuccessfully => "Facilities retrieved successfully";
        
        // Validation Messages
        public static string CodeRequired => "Facility code is required";
        public static string NameRequired => "Facility name is required";
        public static string AreaMustBePositive => "Facility area must be greater than zero";
    }
    
    public static class Equipment
    {
        // Error Messages
        public static string NotFound(string code) => $"Process equipment type with code '{code}' was not found";
        public static string InvalidCode => "Equipment type code cannot be empty";
        public static string InvalidName => "Equipment type name cannot be empty";
        public static string InvalidArea => "Area must be greater than zero";
        
        // Success Messages
        public static string CreatedSuccessfully => "Equipment type created successfully";
        public static string UpdatedSuccessfully => "Equipment type updated successfully";
        public static string DeletedSuccessfully => "Equipment type deleted successfully";
        public static string ListRetrievedSuccessfully => "Equipment types retrieved successfully";
        
        // Validation Messages
        public static string CodeRequired => "Equipment type code is required";
        public static string NameRequired => "Equipment type name is required";
        public static string AreaMustBePositive => "Equipment area must be greater than zero";
    }
    
    public static class Validation
    {
        public static string InvalidInput(string field) => $"Invalid input for field: {field}";
        public static string RequiredField(string field) => $"Field '{field}' is required";
        public static string MaxLength(string field, int maxLength) => 
            $"Field '{field}' must not exceed {maxLength} characters";
        public static string InvalidFormat(string field, string format) => 
            $"Field '{field}' must match format: {format}";
        public static string Failed => "Validation failed";
        
        public static string FieldRequired(string field) => $"{field} is required";
        public static string FieldMaxLength(string field, int maxLength) => 
            $"{field} must not exceed {maxLength} characters";
        public static string FieldMinLength(string field, int minLength) => 
            $"{field} must be at least {minLength} characters";
        public static string FieldInvalidFormat(string field) => 
            $"{field} has invalid format";
        public static string FieldMustBePositive(string field) => 
            $"{field} must be greater than zero";
        public static string FieldMustBeInRange(string field, object min, object max) => 
            $"{field} must be between {min} and {max}";
    }
    
    public static class General
    {
        // Error Messages
        public static string UnexpectedError => "An unexpected error occurred";
        public static string DatabaseError => "A database error occurred";
        public static string ConcurrencyConflict => "The record was modified by another user";
        public static string NotFound => "Resource not found";
        public static string Unauthorized => "Unauthorized access";
        public static string Forbidden => "Access forbidden";
        
        // Success Messages
        public static string OperationSuccessful => "Operation completed successfully";
        public static string DataSaved => "Data saved successfully";
        public static string ChangesApplied => "Changes applied successfully";
    }
    
    public static class Api
    {
        public static string InvalidApiKey => "Invalid API Key";
        public static string ApiKeyNotProvided => "API Key was not provided";
        public static string ApiKeyNotConfigured => "API Key is not configured";
    }
    
    public static class Processing
    {
        public static string ContractCreatedEventProcessed(Guid contractId) => 
            $"Contract created event processed successfully for Contract ID: {contractId}";
        public static string ContractUpdatedEventProcessed(Guid contractId) => 
            $"Contract updated event processed successfully for Contract ID: {contractId}";
        public static string ContractDeactivatedEventProcessed(Guid contractId) => 
            $"Contract deactivated event processed successfully for Contract ID: {contractId}";
    }
}