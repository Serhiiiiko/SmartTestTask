using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Infrastructure.Data.Configurations;

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
        
        // Seed data
        builder.HasData(
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
}