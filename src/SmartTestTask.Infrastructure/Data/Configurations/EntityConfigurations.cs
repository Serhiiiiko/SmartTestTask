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
    }
}

public class ProcessEquipmentTypeConfiguration : IEntityTypeConfiguration<ProcessEquipmentType>
{
    public void Configure(EntityTypeBuilder<ProcessEquipmentType> builder)
    {
        builder.ToTable("ProcessEquipmentTypes");
        
        builder.HasKey(e => e.Code);
        
        builder.Property(e => e.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(e => e.Area)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        // Relationships
        builder.HasMany(e => e.Contracts)
            .WithOne(e => e.ProcessEquipmentType)
            .HasForeignKey(e => e.ProcessEquipmentTypeCode)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(e => e.Name);
    }
}

public class EquipmentPlacementContractConfiguration : IEntityTypeConfiguration<EquipmentPlacementContract>
{
    public void Configure(EntityTypeBuilder<EquipmentPlacementContract> builder)
    {
        builder.ToTable("EquipmentPlacementContracts");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedNever();
        
        builder.Property(e => e.ContractNumber)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(e => e.ProductionFacilityCode)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.ProcessEquipmentTypeCode)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.EquipmentQuantity)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.ModifiedAt);
        
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Relationships
        builder.HasOne(e => e.ProductionFacility)
            .WithMany(e => e.Contracts)
            .HasForeignKey(e => e.ProductionFacilityCode)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.ProcessEquipmentType)
            .WithMany(e => e.Contracts)
            .HasForeignKey(e => e.ProcessEquipmentTypeCode)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(e => e.ContractNumber).IsUnique();
        builder.HasIndex(e => e.ProductionFacilityCode);
        builder.HasIndex(e => e.ProcessEquipmentTypeCode);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.IsActive);
        
        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}