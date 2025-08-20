using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Infrastructure.Data;

namespace SmartTestTask.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);
        
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }
}

public class ProductionFacilityRepository : Repository<ProductionFacility>, IProductionFacilityRepository
{
    public ProductionFacilityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProductionFacility> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Code == code, cancellationToken);
    }

    public async Task<ProductionFacility> GetByCodeWithContractsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Contracts)
                .ThenInclude(c => c.ProcessEquipmentType)
            .FirstOrDefaultAsync(f => f.Code == code, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(f => f.Code == code, cancellationToken);
    }
}

public class ProcessEquipmentTypeRepository : Repository<ProcessEquipmentType>, IProcessEquipmentTypeRepository
{
    public ProcessEquipmentTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProcessEquipmentType> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Code == code, cancellationToken);
    }
}

public class EquipmentPlacementContractRepository : Repository<EquipmentPlacementContract>, IEquipmentPlacementContractRepository
{
    public EquipmentPlacementContractRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EquipmentPlacementContract>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ProductionFacility)
            .Include(c => c.ProcessEquipmentType)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<EquipmentPlacementContract> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ProductionFacility)
            .Include(c => c.ProcessEquipmentType)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EquipmentPlacementContract>> GetByFacilityCodeAsync(string facilityCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ProductionFacility)
            .Include(c => c.ProcessEquipmentType)
            .Where(c => c.ProductionFacilityCode == facilityCode)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EquipmentPlacementContract>> GetActiveContractsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.ProductionFacility)
            .Include(c => c.ProcessEquipmentType)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}