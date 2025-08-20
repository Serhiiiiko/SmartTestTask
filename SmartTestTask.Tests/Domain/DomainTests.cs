using FluentAssertions;
using SmartTestTask.Domain.Entities;

namespace SmartTestTask.Tests.Domain;

public class DomainTests
{
    [Fact]
    public void ProductionFacility_Constructor_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var facility = new ProductionFacility("FAC-001", "Test Facility", 1000m);

        // Assert
        facility.Code.Should().Be("FAC-001");
        facility.Name.Should().Be("Test Facility");
        facility.StandardArea.Should().Be(1000m);
        facility.Contracts.Should().NotBeNull();
        facility.Contracts.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Test", 100)]
    [InlineData("FAC-001", "", 100)]
    [InlineData("FAC-001", "Test", 0)]
    [InlineData("FAC-001", "Test", -100)]
    public void ProductionFacility_Constructor_ShouldThrowException_WhenInvalidData(string code, string name, decimal area)
    {
        // Arrange & Act
        Action act = () => new ProductionFacility(code, name, area);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ProductionFacility_CanAccommodateEquipment_ShouldReturnTrue_WhenEnoughSpace()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Test Facility", 1000m);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 50m);

        // Act
        var result = facility.CanAccommodateEquipment(equipmentType, 10);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ProductionFacility_CanAccommodateEquipment_ShouldReturnFalse_WhenNotEnoughSpace()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Test Facility", 100m);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 50m);

        // Act
        var result = facility.CanAccommodateEquipment(equipmentType, 10);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ProcessEquipmentType_CalculateTotalArea_ShouldReturnCorrectValue()
    {
        // Arrange
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);

        // Act
        var totalArea = equipmentType.CalculateTotalArea(4);

        // Assert
        totalArea.Should().Be(100m);
    }

    [Fact]
    public void ProcessEquipmentType_CalculateTotalArea_ShouldThrowException_WhenQuantityInvalid()
    {
        // Arrange
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);

        // Act
        Action act = () => equipmentType.CalculateTotalArea(0);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EquipmentPlacementContract_Constructor_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);

        // Assert
        contract.Id.Should().NotBeEmpty();
        contract.ProductionFacilityCode.Should().Be("FAC-001");
        contract.ProcessEquipmentTypeCode.Should().Be("EQT-001");
        contract.EquipmentQuantity.Should().Be(5);
        contract.IsActive.Should().BeTrue();
        contract.ContractNumber.Should().NotBeNullOrEmpty();
        contract.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void EquipmentPlacementContract_UpdateQuantity_ShouldUpdateValues()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        contract.ClearDomainEvents();

        // Act
        contract.UpdateQuantity(10);

        // Assert
        contract.EquipmentQuantity.Should().Be(10);
        contract.ModifiedAt.Should().NotBeNull();
        contract.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void EquipmentPlacementContract_Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        contract.ClearDomainEvents();

        // Act
        contract.Deactivate();

        // Assert
        contract.IsActive.Should().BeFalse();
        contract.ModifiedAt.Should().NotBeNull();
        contract.DomainEvents.Should().HaveCount(1);
    }
}