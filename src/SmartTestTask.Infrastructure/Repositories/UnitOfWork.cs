using Microsoft.EntityFrameworkCore.Storage;
using SmartTestTask.Domain.Interfaces;
using SmartTestTask.Infrastructure.Data;

namespace SmartTestTask.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _currentTransaction;
    
    private IProductionFacilityRepository _productionFacilities;
    private IProcessEquipmentTypeRepository _processEquipmentTypes;
    private IEquipmentPlacementContractRepository _equipmentPlacementContracts;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductionFacilityRepository ProductionFacilities
    {
        get
        {
            return _productionFacilities ??= new ProductionFacilityRepository(_context);
        }
    }

    public IProcessEquipmentTypeRepository ProcessEquipmentTypes
    {
        get
        {
            return _processEquipmentTypes ??= new ProcessEquipmentTypeRepository(_context);
        }
    }

    public IEquipmentPlacementContractRepository EquipmentPlacementContracts
    {
        get
        {
            return _equipmentPlacementContracts ??= new EquipmentPlacementContractRepository(_context);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context?.Dispose();
    }
}