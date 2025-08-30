using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Handlers.Commands;
using SmartTestTask.Application.Interfaces;
using SmartTestTask.Application.Types;
using SmartTestTask.Domain.Entities;
using SmartTestTask.Domain.Interfaces;

namespace SmartTestTask.Tests.Application.Handlers;

public class CreateContractCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly Mock<ILogger<CreateContractCommandHandler>> _loggerMock;
    private readonly CreateContractCommandHandler _handler;

    public CreateContractCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _messageBusMock = new Mock<IMessageBusService>();
        _loggerMock = new Mock<ILogger<CreateContractCommandHandler>>();
        
        _handler = new CreateContractCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _messageBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateContractSuccessfully()
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", "EQT-001", 5);
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);
        var expectedContractDto = new ContractDto(
            Guid.NewGuid(),
            "EPC-20250821-12345678",
            "Main Facility",
            "Test Equipment",
            5,
            125m,
            DateTime.UtcNow,
            null,
            true);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync(command.ProductionFacilityCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var equipmentRepoMock = new Mock<IProcessEquipmentTypeRepository>();
        equipmentRepoMock.Setup(x => x.GetByCodeAsync(command.ProcessEquipmentTypeCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipmentType);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdWithIncludesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken ct) => 
            {
                var contract = new EquipmentPlacementContract(command.ProductionFacilityCode, command.ProcessEquipmentTypeCode, command.EquipmentQuantity);
                // Set navigation properties using reflection
                SetPrivateProperty(contract, "ProductionFacility", facility);
                SetPrivateProperty(contract, "ProcessEquipmentType", equipmentType);
                return contract;
            });

        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProcessEquipmentTypes).Returns(equipmentRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<ContractDto>(It.IsAny<EquipmentPlacementContract>()))
            .Returns(expectedContractDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedContractDto);

        contractRepoMock.Verify(x => x.AddAsync(It.IsAny<EquipmentPlacementContract>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFacilityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateContractCommand("FAC-999", "EQT-001", 5);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync(command.ProductionFacilityCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductionFacility)null);

        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Facility.NotFound");
        result.Error.Message.Should().Contain("FAC-999");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEquipmentTypeNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", "EQT-999", 5);
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync(command.ProductionFacilityCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var equipmentRepoMock = new Mock<IProcessEquipmentTypeRepository>();
        equipmentRepoMock.Setup(x => x.GetByCodeAsync(command.ProcessEquipmentTypeCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcessEquipmentType)null);

        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProcessEquipmentTypes).Returns(equipmentRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Equipment.NotFound");
        result.Error.Message.Should().Contain("EQT-999");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInsufficientArea_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", "EQT-001", 50);
        var facility = new ProductionFacility("FAC-001", "Main Facility", 100m);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync(command.ProductionFacilityCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var equipmentRepoMock = new Mock<IProcessEquipmentTypeRepository>();
        equipmentRepoMock.Setup(x => x.GetByCodeAsync(command.ProcessEquipmentTypeCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipmentType);

        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProcessEquipmentTypes).Returns(equipmentRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Facility.InsufficientArea");
        result.Error.Message.Should().Contain("Required: 1250");
        result.Error.Message.Should().Contain("Available: 100");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        propertyInfo?.SetValue(obj, value);
    }
}

public class UpdateContractCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly Mock<ILogger<UpdateContractCommandHandler>> _loggerMock;
    private readonly UpdateContractCommandHandler _handler;

    public UpdateContractCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _messageBusMock = new Mock<IMessageBusService>();
        _loggerMock = new Mock<ILogger<UpdateContractCommandHandler>>();
        
        _handler = new UpdateContractCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _messageBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateContractSuccessfully()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new UpdateContractCommand(contractId, 10);
        
        var facility = new ProductionFacility("FAC-001", "Main Facility", 1000m);
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);
        var existingContract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        
        SetPrivateProperty(existingContract, "Id", contractId);
        SetPrivateProperty(existingContract, "ProductionFacility", facility);
        SetPrivateProperty(existingContract, "ProcessEquipmentType", equipmentType);

        var expectedContractDto = new ContractDto(
            contractId,
            "EPC-20250821-12345678",
            "Main Facility",
            "Test Equipment",
            10,
            250m,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow,
            true);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdWithIncludesAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingContract);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync("FAC-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var equipmentRepoMock = new Mock<IProcessEquipmentTypeRepository>();
        equipmentRepoMock.Setup(x => x.GetByCodeAsync("EQT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipmentType);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProcessEquipmentTypes).Returns(equipmentRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<ContractDto>(It.IsAny<EquipmentPlacementContract>()))
            .Returns(expectedContractDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedContractDto);
        result.Value.EquipmentQuantity.Should().Be(10);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractUpdatedMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenContractNotFound_ShouldReturnFailure()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new UpdateContractCommand(contractId, 10);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdWithIncludesAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EquipmentPlacementContract)null);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Contract.NotFound");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractUpdatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInsufficientAreaForIncrease_ShouldReturnFailure()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new UpdateContractCommand(contractId, 50); // Increasing from 5 to 50
        
        var facility = new ProductionFacility("FAC-001", "Main Facility", 200m); // Small facility
        var equipmentType = new ProcessEquipmentType("EQT-001", "Test Equipment", 25m);
        var existingContract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        
        SetPrivateProperty(existingContract, "Id", contractId);
        SetPrivateProperty(existingContract, "ProductionFacility", facility);
        SetPrivateProperty(existingContract, "ProcessEquipmentType", equipmentType);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdWithIncludesAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingContract);

        var facilityRepoMock = new Mock<IProductionFacilityRepository>();
        facilityRepoMock.Setup(x => x.GetByCodeWithContractsAsync("FAC-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var equipmentRepoMock = new Mock<IProcessEquipmentTypeRepository>();
        equipmentRepoMock.Setup(x => x.GetByCodeAsync("EQT-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipmentType);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProductionFacilities).Returns(facilityRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.ProcessEquipmentTypes).Returns(equipmentRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Facility.InsufficientArea");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        propertyInfo?.SetValue(obj, value);
    }
}

public class DeactivateContractCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessageBusService> _messageBusMock;
    private readonly Mock<ILogger<DeactivateContractCommandHandler>> _loggerMock;
    private readonly DeactivateContractCommandHandler _handler;

    public DeactivateContractCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messageBusMock = new Mock<IMessageBusService>();
        _loggerMock = new Mock<ILogger<DeactivateContractCommandHandler>>();
        
        _handler = new DeactivateContractCommandHandler(
            _unitOfWorkMock.Object,
            _messageBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithActiveContract_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new DeactivateContractCommand(contractId);
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        SetPrivateProperty(contract, "Id", contractId);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        contract.IsActive.Should().BeFalse();
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractDeactivatedMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenContractNotFound_ShouldReturnFailure()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new DeactivateContractCommand(contractId);

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EquipmentPlacementContract)null);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Contract.NotFound");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractDeactivatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenContractAlreadyDeactivated_ShouldReturnFailure()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var command = new DeactivateContractCommand(contractId);
        var contract = new EquipmentPlacementContract("FAC-001", "EQT-001", 5);
        SetPrivateProperty(contract, "Id", contractId);
        contract.Deactivate(); // Already deactivated

        var contractRepoMock = new Mock<IEquipmentPlacementContractRepository>();
        contractRepoMock.Setup(x => x.GetByIdAsync(contractId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _unitOfWorkMock.Setup(x => x.EquipmentPlacementContracts).Returns(contractRepoMock.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Contract.AlreadyDeactivated");
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _messageBusMock.Verify(x => x.SendMessageAsync(It.IsAny<ContractDeactivatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        propertyInfo?.SetValue(obj, value);
    }
}