using SmartTestTask.Application.Resources;

namespace SmartTestTask.Application.Constants;

public static class MessageConstants
{
    // Contract Messages
    public static class Contract
    {
        // Error Messages
        public static string NotFound(Guid id) => string.Format(ErrorMessages.Contract_NotFound, id);
        public static string InvalidQuantity => ErrorMessages.Contract_InvalidQuantity;
        public static string AlreadyDeactivated => ErrorMessages.Contract_AlreadyDeactivated;
        public static string CreationFailed => ErrorMessages.Contract_CreationFailed;
        public static string UpdateFailed => ErrorMessages.Contract_UpdateFailed;
        public static string DeactivationFailed => ErrorMessages.Contract_DeactivationFailed;
        public static string IdMismatch => ErrorMessages.Contract_IdMismatch;
        
        // Success Messages
        public static string CreatedSuccessfully => SuccessMessages.Contract_CreatedSuccessfully;
        public static string UpdatedSuccessfully => SuccessMessages.Contract_UpdatedSuccessfully;
        public static string DeactivatedSuccessfully => SuccessMessages.Contract_DeactivatedSuccessfully;
        public static string RetrievedSuccessfully => SuccessMessages.Contract_RetrievedSuccessfully;
        public static string ListRetrievedSuccessfully => SuccessMessages.Contracts_RetrievedSuccessfully;
        
        // Validation Messages
        public static string FacilityCodeRequired => ValidationMessages.Contract_FacilityCodeRequired;
        public static string FacilityCodeMaxLength => ValidationMessages.Contract_FacilityCodeMaxLength;
        public static string FacilityCodeFormat => ValidationMessages.Contract_FacilityCodeFormat;
        public static string EquipmentCodeRequired => ValidationMessages.Contract_EquipmentCodeRequired;
        public static string EquipmentCodeMaxLength => ValidationMessages.Contract_EquipmentCodeMaxLength;
        public static string EquipmentCodeFormat => ValidationMessages.Contract_EquipmentCodeFormat;
        public static string QuantityRequired => ValidationMessages.Contract_QuantityRequired;
        public static string QuantityMustBePositive => ValidationMessages.Contract_QuantityMustBePositive;
        public static string QuantityMaximum => ValidationMessages.Contract_QuantityMaximum;
        public static string IdRequired => ValidationMessages.Contract_IdRequired;
    }
    
    public static class Facility
    {
        // Error Messages
        public static string NotFound(string code) => string.Format(ErrorMessages.Facility_NotFound, code);
        public static string InsufficientArea(string code, decimal required, decimal available) =>
            string.Format(ErrorMessages.Facility_InsufficientArea, code, required, available);
        public static string InvalidCode => ErrorMessages.Facility_InvalidCode;
        public static string InvalidName => ErrorMessages.Facility_InvalidName;
        public static string InvalidArea => ErrorMessages.Facility_InvalidArea;
        
        // Success Messages
        public static string CreatedSuccessfully => SuccessMessages.Facility_CreatedSuccessfully;
        public static string UpdatedSuccessfully => SuccessMessages.Facility_UpdatedSuccessfully;
        public static string DeletedSuccessfully => SuccessMessages.Facility_DeletedSuccessfully;
        public static string ListRetrievedSuccessfully => SuccessMessages.Facilities_RetrievedSuccessfully;
        
        // Validation Messages
        public static string CodeRequired => ValidationMessages.Facility_CodeRequired;
        public static string NameRequired => ValidationMessages.Facility_NameRequired;
        public static string AreaMustBePositive => ValidationMessages.Facility_AreaMustBePositive;
    }
    
    public static class Equipment
    {
        // Error Messages
        public static string NotFound(string code) => string.Format(ErrorMessages.Equipment_NotFound, code);
        public static string InvalidCode => ErrorMessages.Equipment_InvalidCode;
        public static string InvalidName => ErrorMessages.Equipment_InvalidName;
        public static string InvalidArea => ErrorMessages.Equipment_InvalidArea;
        
        // Success Messages
        public static string CreatedSuccessfully => SuccessMessages.Equipment_CreatedSuccessfully;
        public static string UpdatedSuccessfully => SuccessMessages.Equipment_UpdatedSuccessfully;
        public static string DeletedSuccessfully => SuccessMessages.Equipment_DeletedSuccessfully;
        public static string ListRetrievedSuccessfully => SuccessMessages.EquipmentTypes_RetrievedSuccessfully;
        
        // Validation Messages
        public static string CodeRequired => ValidationMessages.Equipment_CodeRequired;
        public static string NameRequired => ValidationMessages.Equipment_NameRequired;
        public static string AreaMustBePositive => ValidationMessages.Equipment_AreaMustBePositive;
    }
    
    public static class Validation
    {
        public static string InvalidInput(string field) => string.Format(ErrorMessages.Validation_InvalidInput, field);
        public static string RequiredField(string field) => string.Format(ErrorMessages.Validation_RequiredField, field);
        public static string MaxLength(string field, int maxLength) => 
            string.Format(ErrorMessages.Validation_MaxLength, field, maxLength);
        public static string InvalidFormat(string field, string format) => 
            string.Format(ErrorMessages.Validation_InvalidFormat, field, format);
        public static string Failed => ErrorMessages.Validation_Failed;
        
        public static string FieldRequired(string field) => string.Format(ValidationMessages.Field_Required, field);
        public static string FieldMaxLength(string field, int maxLength) => 
            string.Format(ValidationMessages.Field_MaxLength, field, maxLength);
        public static string FieldMinLength(string field, int minLength) => 
            string.Format(ValidationMessages.Field_MinLength, field, minLength);
        public static string FieldInvalidFormat(string field) => 
            string.Format(ValidationMessages.Field_InvalidFormat, field);
        public static string FieldMustBePositive(string field) => 
            string.Format(ValidationMessages.Field_MustBePositive, field);
        public static string FieldMustBeInRange(string field, object min, object max) => 
            string.Format(ValidationMessages.Field_MustBeInRange, field, min, max);
    }
    
    public static class General
    {
        // Error Messages
        public static string UnexpectedError => ErrorMessages.General_UnexpectedError;
        public static string DatabaseError => ErrorMessages.General_DatabaseError;
        public static string ConcurrencyConflict => ErrorMessages.General_ConcurrencyConflict;
        public static string NotFound => ErrorMessages.General_NotFound;
        public static string Unauthorized => ErrorMessages.General_Unauthorized;
        public static string Forbidden => ErrorMessages.General_Forbidden;
        
        // Success Messages
        public static string OperationSuccessful => SuccessMessages.General_OperationSuccessful;
        public static string DataSaved => SuccessMessages.General_DataSaved;
        public static string ChangesApplied => SuccessMessages.General_ChangesApplied;
    }
    
    public static class Api
    {
        public static string InvalidApiKey => ErrorMessages.Api_InvalidApiKey;
        public static string ApiKeyNotProvided => ErrorMessages.Api_ApiKeyNotProvided;
        public static string ApiKeyNotConfigured => ErrorMessages.Api_ApiKeyNotConfigured;
    }
    
    public static class Processing
    {
        public static string ContractCreatedEventProcessed(Guid contractId) => 
            string.Format(SuccessMessages.Processing_ContractCreatedEventProcessed, contractId);
        public static string ContractUpdatedEventProcessed(Guid contractId) => 
            string.Format(SuccessMessages.Processing_ContractUpdatedEventProcessed, contractId);
        public static string ContractDeactivatedEventProcessed(Guid contractId) => 
            string.Format(SuccessMessages.Processing_ContractDeactivatedEventProcessed, contractId);
    }
}