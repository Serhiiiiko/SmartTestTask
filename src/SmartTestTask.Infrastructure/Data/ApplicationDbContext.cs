using Microsoft.EntityFrameworkCore;
using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ProductionFacility> ProductionFacilities { get; set; }
    public DbSet<ProcessEquipmentType> ProcessEquipmentTypes { get; set; }
    public DbSet<EquipmentPlacementContract> EquipmentPlacementContracts { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        foreach (var entry in ChangeTracker.Entries<EquipmentPlacementContract>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.GetType().GetProperty("ModifiedAt")?.SetValue(entry.Entity, DateTime.UtcNow);
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}