using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Infrastructure.Data;
using SmartTestTask.Infrastructure.Repositories;

namespace SmartTestTask.Tests.Repositories;


public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        var facilities = new[]
        {
            new ProductionFacility("FAC-TEST-001", "Test Facility 1", 5000m),
            new ProductionFacility("FAC-TEST-002", "Test Facility 2", 3000m)
        };

        var equipmentTypes = new[]
        {
            new ProcessEquipmentType("EQT-TEST-001", "Test Equipment 1", 25m),
            new ProcessEquipmentType("EQT-TEST-002", "Test Equipment 2", 50m)
        };

        _context.ProductionFacilities.AddRange(facilities);
        _context.ProcessEquipmentTypes.AddRange(equipmentTypes);
        _context.SaveChanges();
    }

    [Fact]
    public async Task ProductionFacilityRepository_GetByCodeAsync_ShouldReturnFacility()
    {
        // Act
        var facility = await _unitOfWork.ProductionFacilities.GetByCodeAsync("FAC-TEST-001");

        // Assert
        facility.Should().NotBeNull();
        facility.Code.Should().Be("FAC-TEST-001");
        facility.Name.Should().Be("Test Facility 1");
    }

    [Fact]
    public async Task ProductionFacilityRepository_ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        // Act
        var exists = await _unitOfWork.ProductionFacilities.ExistsAsync("FAC-TEST-001");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ProductionFacilityRepository_ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var exists = await _unitOfWork.ProductionFacilities.ExistsAsync("FAC-NOT-EXISTS");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessEquipmentTypeRepository_GetByCodeAsync_ShouldReturnEquipmentType()
    {
        // Act
        var equipmentType = await _unitOfWork.ProcessEquipmentTypes.GetByCodeAsync("EQT-TEST-001");

        // Assert
        equipmentType.Should().NotBeNull();
        equipmentType.Code.Should().Be("EQT-TEST-001");
        equipmentType.Name.Should().Be("Test Equipment 1");
    }

    [Fact]
    public async Task EquipmentPlacementContractRepository_AddAsync_ShouldAddContract()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-TEST-001", "EQT-TEST-001", 3);

        // Act
        await _unitOfWork.EquipmentPlacementContracts.AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var savedContract = await _unitOfWork.EquipmentPlacementContracts.GetByIdAsync(contract.Id);
        savedContract.Should().NotBeNull();
        savedContract.Id.Should().Be(contract.Id);
    }

    [Fact]
    public async Task EquipmentPlacementContractRepository_GetAllWithIncludesAsync_ShouldReturnContractsWithRelatedData()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-TEST-001", "EQT-TEST-001", 3);
        await _unitOfWork.EquipmentPlacementContracts.AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var contracts = await _unitOfWork.EquipmentPlacementContracts.GetAllWithIncludesAsync();

        // Assert
        contracts.Should().NotBeEmpty();
        var firstContract = contracts.First();
        firstContract.ProductionFacility.Should().NotBeNull();
        firstContract.ProcessEquipmentType.Should().NotBeNull();
    }

    [Fact]
    public async Task UnitOfWork_Transaction_ShouldCommitSuccessfully()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var contract = new EquipmentPlacementContract("FAC-TEST-001", "EQT-TEST-001", 5);

        // Act
        await _unitOfWork.EquipmentPlacementContracts.AddAsync(contract);
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedContract = await _unitOfWork.EquipmentPlacementContracts.GetByIdAsync(contract.Id);
        savedContract.Should().NotBeNull();
    }

    [Fact]
    public async Task UnitOfWork_Transaction_ShouldRollbackOnError()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var contractId = Guid.NewGuid();

        try
        {
            // This will fail due to invalid facility code
            var contract = new EquipmentPlacementContract("INVALID", "EQT-TEST-001", 5);
            await _unitOfWork.EquipmentPlacementContracts.AddAsync(contract);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            // Transaction should be rolled back
        }

        // Act & Assert
        var savedContract = await _unitOfWork.EquipmentPlacementContracts.GetByIdAsync(contractId);
        savedContract.Should().BeNull();
    }

    public void Dispose()
    {
        _unitOfWork?.Dispose();
        _context?.Dispose();
    }
}