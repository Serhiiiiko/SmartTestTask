using SmartTestTask.Domain.Entities;
using System.Linq.Expressions;

namespace SmartTestTask.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

public interface IProductionFacilityRepository : IRepository<ProductionFacility>
{
    Task<ProductionFacility> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ProductionFacility> GetByCodeWithContractsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}

public interface IProcessEquipmentTypeRepository : IRepository<ProcessEquipmentType>
{
    Task<ProcessEquipmentType> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}

public interface IEquipmentPlacementContractRepository : IRepository<EquipmentPlacementContract>
{
    Task<IEnumerable<EquipmentPlacementContract>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default);
    Task<EquipmentPlacementContract> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentPlacementContract>> GetByFacilityCodeAsync(string facilityCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentPlacementContract>> GetActiveContractsAsync(CancellationToken cancellationToken = default);
}

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