using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Domain.Interfaces;

public interface IProcessEquipmentTypeRepository : IRepository<ProcessEquipmentType>
{
    Task<ProcessEquipmentType> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
    
}