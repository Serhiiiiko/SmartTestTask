using Microsoft.EntityFrameworkCore;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Infrastructure.Data.Configurations;

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
        
        // Apply configurations
        modelBuilder.ApplyConfiguration(new ProductionFacilityConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessEquipmentTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentPlacementContractConfiguration());
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Production Facilities
        modelBuilder.Entity<ProductionFacility>().HasData(
            new { Code = "FAC-001", Name = "Main Production Facility", StandardArea = 10000m },
            new { Code = "FAC-002", Name = "Secondary Production Facility", StandardArea = 7500m },
            new { Code = "FAC-003", Name = "North Wing Facility", StandardArea = 5000m },
            new { Code = "FAC-004", Name = "South Wing Facility", StandardArea = 5000m },
            new { Code = "FAC-005", Name = "Research & Development Facility", StandardArea = 3000m }
        );
        
        // Seed Process Equipment Types
        modelBuilder.Entity<ProcessEquipmentType>().HasData(
            new { Code = "EQT-001", Name = "Industrial Lathe", Area = 25m },
            new { Code = "EQT-002", Name = "CNC Milling Machine", Area = 30m },
            new { Code = "EQT-003", Name = "3D Printer", Area = 10m },
            new { Code = "EQT-004", Name = "Assembly Robot", Area = 40m },
            new { Code = "EQT-005", Name = "Quality Control Station", Area = 15m },
            new { Code = "EQT-006", Name = "Packaging Machine", Area = 35m },
            new { Code = "EQT-007", Name = "Welding Station", Area = 20m },
            new { Code = "EQT-008", Name = "Paint Booth", Area = 50m },
            new { Code = "EQT-009", Name = "Testing Equipment", Area = 18m },
            new { Code = "EQT-010", Name = "Storage Rack System", Area = 60m }
        );
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