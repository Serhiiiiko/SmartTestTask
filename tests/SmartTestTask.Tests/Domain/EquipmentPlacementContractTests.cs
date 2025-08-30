using FluentAssertions;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Domain.Events;

namespace SmartTestTask.Tests.Domain;

public class EquipmentPlacementContractTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateContract()
    {
        // Arrange
        var facilityCode = "FAC-001";
        var equipmentCode = "EQT-001";
        var quantity = 5;

        // Act
        var contract = new EquipmentPlacementContract(facilityCode, equipmentCode, quantity);

        // Assert
        contract.Should().NotBeNull();
        contract.Id.Should().NotBeEmpty();
        contract.ProductionFacilityCode.Should().Be(facilityCode);
        contract.ProcessEquipmentTypeCode.Should().Be(equipmentCode);
        contract.EquipmentQuantity.Should().Be(quantity);
        contract.IsActive.Should().BeTrue();
        contract.ContractNumber.Should().NotBeNullOrWhiteSpace();
        contract.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        contract.ModifiedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(null, "EQT-001", 5)]
    [InlineData("", "EQT-001", 5)]
    [InlineData(" ", "EQT-001", 5)]
    [InlineData("FAC-001", null, 5)]
    [InlineData("FAC-001", "", 5)]
    [InlineData("FAC-001", " ", 5)]
    public void Constructor_WithInvalidCodes_ShouldThrowArgumentException(string facilityCode, string equipmentCode, int quantity)
    {
        // Act & Assert
        var act = () => new EquipmentPlacementContract(facilityCode, equipmentCode, quantity);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidQuantity_ShouldThrowArgumentException(int quantity)
    {
        // Act & Assert
        var act = () => new EquipmentPlacementContract("FAC-001", "EQT-001", quantity);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*quantity must be greater than zero*");
    }

    [Fact]
    public void Constructor_ShouldRaiseContractCreatedEvent()
    {
        // Arrange & Act
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);

        // Assert
        contract.DomainEvents.Should().HaveCount(1);
        var domainEvent = contract.DomainEvents.First();
        domainEvent.Should().BeOfType<ContractCreatedEvent>();
        
        var createdEvent = domainEvent as ContractCreatedEvent;
        createdEvent.ContractId.Should().Be(contract.Id);
        createdEvent.FacilityCode.Should().Be("FAC-001");
        createdEvent.EquipmentTypeCode.Should().Be("EQT-001");
        createdEvent.Quantity.Should().Be(5);
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateAndRaiseEvent()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        contract.ClearDomainEvents(); // Clear creation event
        var newQuantity = 10;

        // Act
        contract.UpdateQuantity(newQuantity);

        // Assert
        contract.EquipmentQuantity.Should().Be(newQuantity);
        contract.ModifiedAt.Should().NotBeNull();
        contract.ModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        contract.DomainEvents.Should().HaveCount(1);
        var updateEvent = contract.DomainEvents.First() as ContractUpdatedEvent;
        updateEvent.Should().NotBeNull();
        updateEvent.ContractId.Should().Be(contract.Id);
        updateEvent.NewQuantity.Should().Be(newQuantity);
    }

    [Fact]
    public void UpdateQuantity_WithSameQuantity_ShouldNotUpdateOrRaiseEvent()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        contract.ClearDomainEvents();

        // Act
        contract.UpdateQuantity(5);

        // Assert
        contract.ModifiedAt.Should().BeNull();
        contract.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateQuantity_WithInvalidQuantity_ShouldThrowException(int newQuantity)
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);

        // Act & Assert
        var act = () => contract.UpdateQuantity(newQuantity);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivateAndRaiseEvent()
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
        var deactivatedEvent = contract.DomainEvents.First() as ContractDeactivatedEvent;
        deactivatedEvent.Should().NotBeNull();
        deactivatedEvent.ContractId.Should().Be(contract.Id);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldNotRaiseEvent()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        contract.Deactivate();
        contract.ClearDomainEvents();

        // Act
        contract.Deactivate();

        // Assert
        contract.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void GetTotalArea_WithProcessEquipmentType_ShouldCalculateCorrectly()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);
        
        // Use reflection to set the navigation property (since it's private set)
        var propertyInfo = typeof(EquipmentPlacementContract).GetProperty("ProcessEquipmentType");
        propertyInfo.SetValue(contract, equipmentType);

        // Act
        var totalArea = contract.GetTotalArea();

        // Assert
        totalArea.Should().Be(125m); // 25 * 5
    }

    [Fact]
    public void GetTotalArea_WithoutProcessEquipmentType_ShouldReturnZero()
    {
        // Arrange
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);

        // Act
        var totalArea = contract.GetTotalArea();

        // Assert
        totalArea.Should().Be(0m);
    }

    [Fact]
    public void ContractNumber_ShouldFollowExpectedFormat()
    {
        // Arrange & Act
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);

        // Assert
        contract.ContractNumber.Should().StartWith("EPC-");
        contract.ContractNumber.Should().MatchRegex(@"^EPC-\d{8}-[A-Z0-9]{8}$");
    }
}

public class ProductionFacilityTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateFacility()
    {
        // Arrange & Act
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);

        // Assert
        facility.Should().NotBeNull();
        facility.Code.Should().Be("FAC-001");
        facility.Name.Should().Be("Main Facility");
        facility.StandardArea.Should().Be(1000m);
        facility.Contracts.Should().NotBeNull();
        facility.Contracts.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, "Name", 100)]
    [InlineData("", "Name", 100)]
    [InlineData(" ", "Name", 100)]
    [InlineData("CODE", null, 100)]
    [InlineData("CODE", "", 100)]
    [InlineData("CODE", " ", 100)]
    public void Constructor_WithInvalidStrings_ShouldThrowException(string code, string name, decimal area)
    {
        // Act & Assert
        var act = () => new ProductionFacility(code, name, area);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidArea_ShouldThrowException(decimal area)
    {
        // Act & Assert
        var act = () => new ProductionFacility("FAC-001", "Facility", area);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Standard area must be greater than zero*");
    }

    [Fact]
    public void GetOccupiedArea_WithContracts_ShouldCalculateCorrectly()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipment1 = new ProcessEquipmentType("EQT-001", "Equipment 1", 25m);
        var equipment2 = new ProcessEquipmentType("EQT-002", "Equipment 2", 30m);
        
        var contract1 = new EquipmentPlacementContract("FAC-001", "EQT-001", 2);
        var contract2 = new EquipmentPlacementContract("FAC-001", "EQT-002", 3);
        
        // Set up the relationships using reflection
        SetPrivateProperty(contract1, "ProcessEquipmentType", equipment1);
        SetPrivateProperty(contract2, "ProcessEquipmentType", equipment2);
        
        facility.Contracts.Add(contract1);
        facility.Contracts.Add(contract2);

        // Act
        var occupiedArea = facility.GetOccupiedArea();

        // Assert
        occupiedArea.Should().Be(140m); // (25 * 2) + (30 * 3)
    }

    [Fact]
    public void GetOccupiedArea_WithoutContracts_ShouldReturnZero()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);

        // Act
        var occupiedArea = facility.GetOccupiedArea();

        // Assert
        occupiedArea.Should().Be(0m);
    }

    [Fact]
    public void GetAvailableArea_ShouldCalculateCorrectly()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipment = new ProcessEquipmentType("EQT-001", "Equipment", 100m);
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 3);
        
        SetPrivateProperty(contract, "ProcessEquipmentType", equipment);
        facility.Contracts.Add(contract);

        // Act
        var availableArea = facility.GetAvailableArea();

        // Assert
        availableArea.Should().Be(700m); // 1000 - 300
    }

    [Fact]
    public void CanAccommodateEquipment_WhenSufficientSpace_ShouldReturnTrue()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipment = new ProcessEquipmentType("EQT-001", "Equipment", 100m);

        // Act
        var canAccommodate = facility.CanAccommodateEquipment(equipment, 5);

        // Assert
        canAccommodate.Should().BeTrue();
    }

    [Fact]
    public void CanAccommodateEquipment_WhenInsufficientSpace_ShouldReturnFalse()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipment = new ProcessEquipmentType("EQT-001", "Equipment", 100m);

        // Act
        var canAccommodate = facility.CanAccommodateEquipment(equipment, 11);

        // Assert
        canAccommodate.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CanAccommodateEquipment_WithInvalidQuantity_ShouldReturnFalse(int quantity)
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipment = new ProcessEquipmentType("EQT-001", "Equipment", 100m);

        // Act
        var canAccommodate = facility.CanAccommodateEquipment(equipment, quantity);

        // Assert
        canAccommodate.Should().BeFalse();
    }

    [Fact]
    public void CanAccommodateEquipment_WithNullEquipment_ShouldReturnFalse()
    {
        // Arrange
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);

        // Act
        var canAccommodate = facility.CanAccommodateEquipment(null, 5);

        // Assert
        canAccommodate.Should().BeFalse();
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        propertyInfo.SetValue(obj, value);
    }
}

public class ProcessEquipmentTypeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEquipmentType()
    {
        // Arrange & Act
        var equipment = new ProcessEquipmentType("EQT-001", "Test Equipment", 50m);

        // Assert
        equipment.Should().NotBeNull();
        equipment.Code.Should().Be("EQT-001");
        equipment.Name.Should().Be("Test Equipment");
        equipment.Area.Should().Be(50m);
        equipment.Contracts.Should().NotBeNull();
        equipment.Contracts.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, "Name", 100)]
    [InlineData("", "Name", 100)]
    [InlineData(" ", "Name", 100)]
    [InlineData("CODE", null, 100)]
    [InlineData("CODE", "", 100)]
    [InlineData("CODE", " ", 100)]
    public void Constructor_WithInvalidStrings_ShouldThrowException(string code, string name, decimal area)
    {
        // Act & Assert
        var act = () => new ProcessEquipmentType(code, name, area);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidArea_ShouldThrowException(decimal area)
    {
        // Act & Assert
        var act = () => new ProcessEquipmentType("EQT-001", "Equipment", area);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Area must be greater than zero*");
    }

    [Fact]
    public void CalculateTotalArea_WithValidQuantity_ShouldCalculateCorrectly()
    {
        // Arrange
        var equipment = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);

        // Act
        var totalArea = equipment.CalculateTotalArea(4);

        // Assert
        totalArea.Should().Be(100m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateTotalArea_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        // Arrange
        var equipment = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);

        // Act & Assert
        var act = () => equipment.CalculateTotalArea(quantity);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
    }
}