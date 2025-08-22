namespace SmartTestTask.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductionFacilityRepository ProductionFacilities { get; }
    IProcessEquipmentTypeRepository ProcessEquipmentTypes { get; }
    IEquipmentPlacementContractRepository EquipmentPlacementContracts { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}