using SmartTestTask.Application.Types;

namespace SmartTestTask.Application.Interfaces;

public interface IValidationService
{
    Task<bool> ValidateContractCreationAsync(string facilityCode, string equipmentTypeCode, int quantity, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateAsync<T>(T model, CancellationToken cancellationToken = default) where T : class;
}