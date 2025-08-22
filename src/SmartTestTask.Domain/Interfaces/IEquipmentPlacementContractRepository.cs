using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Domain.Interfaces;

public interface IEquipmentPlacementContractRepository : IRepository<EquipmentPlacementContract>
{
    Task<IEnumerable<EquipmentPlacementContract>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default);
    Task<EquipmentPlacementContract> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentPlacementContract>> GetByFacilityCodeAsync(string facilityCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentPlacementContract>> GetActiveContractsAsync(CancellationToken cancellationToken = default);
}