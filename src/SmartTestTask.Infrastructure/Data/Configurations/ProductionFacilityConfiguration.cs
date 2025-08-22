using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Infrastructure.Data.Configurations;

public class ProductionFacilityConfiguration : IEntityTypeConfiguration<ProductionFacility>
{
    public void Configure(EntityTypeBuilder<ProductionFacility> builder)
    {
        builder.ToTable("ProductionFacilities");
        
        builder.HasKey(e => e.Code);
        
        builder.Property(e => e.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(e => e.StandardArea)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        // Relationships
        builder.HasMany(e => e.Contracts)
            .WithOne(e => e.ProductionFacility)
            .HasForeignKey(e => e.ProductionFacilityCode)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(e => e.Name);
        
        // Seed data
        builder.HasData(
            new { Code = "FAC-001", Name = "Main Production Facility", StandardArea = 10000m },
            new { Code = "FAC-002", Name = "Secondary Production Facility", StandardArea = 7500m },
            new { Code = "FAC-003", Name = "North Wing Facility", StandardArea = 5000m },
            new { Code = "FAC-004", Name = "South Wing Facility", StandardArea = 5000m },
            new { Code = "FAC-005", Name = "Research & Development Facility", StandardArea = 3000m }
        );
    }
}