using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Domain.Interfaces;

public interface IProductionFacilityRepository : IRepository<ProductionFacility>
{
    Task<ProductionFacility> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ProductionFacility> GetByCodeWithContractsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}