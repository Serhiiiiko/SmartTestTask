using Microsoft.EntityFrameworkCore.Storage;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Infrastructure.Data;

namespace SmartTestTask.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _currentTransaction;
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        ProductionFacilities = new ProductionFacilityRepository(context);
        ProcessEquipmentTypes = new ProcessEquipmentTypeRepository(context);
        EquipmentPlacementContracts = new EquipmentPlacementContractRepository(context);
    }

    public IProductionFacilityRepository ProductionFacilities { get; }
    public IProcessEquipmentTypeRepository ProcessEquipmentTypes { get; }
    public IEquipmentPlacementContractRepository EquipmentPlacementContracts { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}