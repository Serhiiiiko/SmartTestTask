using FluentAssertions;
using FluentValidation.TestHelper;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Request;
using SmartTestTask.Application.Validators;

namespace SmartTestTask.Tests.Application.Validators;

public class CreateContractCommandValidatorTests
{
    private readonly CreateContractCommandValidator _validator;

    public CreateContractCommandValidatorTests()
    {
        _validator = new CreateContractCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new CreateContractCommand(
            "FAC-001",
            "EQT-001",
            10
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_When_ProductionFacilityCodeIsEmpty(string facilityCode)
    {
        // Arrange
        var command = new CreateContractCommand(facilityCode, "EQT-001", 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode)
            .WithErrorMessage("Production facility code is required");
    }

    [Fact]
    public void Should_HaveError_When_ProductionFacilityCodeExceedsMaxLength()
    {
        // Arrange
        var longCode = new string('A', 51);
        var command = new CreateContractCommand(longCode, "EQT-001", 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode)
            .WithErrorMessage("Production facility code must not exceed 50 characters");
    }

    [Theory]
    [InlineData("fac-001")] // lowercase
    [InlineData("FAC 001")] // space
    [InlineData("FAC@001")] // special character
    [InlineData("FAC_001")] // underscore
    public void Should_HaveError_When_ProductionFacilityCodeHasInvalidFormat(string facilityCode)
    {
        // Arrange
        var command = new CreateContractCommand(facilityCode, "EQT-001", 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode)
            .WithErrorMessage("Production facility code must contain only uppercase letters, numbers, and hyphens");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_When_ProcessEquipmentTypeCodeIsEmpty(string equipmentCode)
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", equipmentCode, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProcessEquipmentTypeCode)
            .WithErrorMessage("Process equipment type code is required");
    }

    [Fact]
    public void Should_HaveError_When_ProcessEquipmentTypeCodeExceedsMaxLength()
    {
        // Arrange
        var longCode = new string('A', 51);
        var command = new CreateContractCommand("FAC-001", longCode, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProcessEquipmentTypeCode)
            .WithErrorMessage("Process equipment type code must not exceed 50 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Should_HaveError_When_EquipmentQuantityIsNotPositive(int quantity)
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", "EQT-001", quantity);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must be greater than zero");
    }

    [Fact]
    public void Should_HaveError_When_EquipmentQuantityExceedsMaximum()
    {
        // Arrange
        var command = new CreateContractCommand("FAC-001", "EQT-001", 1001);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must not exceed 1000");
    }

    [Theory]
    [InlineData("FAC-001", 1)]
    [InlineData("FAC-002", 100)]
    [InlineData("FAC-003", 1000)]
    [InlineData("A", 50)]
    [InlineData("ABC123", 50)]
    [InlineData("TEST-FACILITY-CODE-123", 50)]
    public void Should_Pass_With_ValidCodes_And_Quantities(string facilityCode, int quantity)
    {
        // Arrange
        var command = new CreateContractCommand(facilityCode, "EQT-001", quantity);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProductionFacilityCode);
        result.ShouldNotHaveValidationErrorFor(x => x.EquipmentQuantity);
    }
}

public class UpdateContractCommandValidatorTests
{
    private readonly UpdateContractCommandValidator _validator;

    public UpdateContractCommandValidatorTests()
    {
        _validator = new UpdateContractCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new UpdateContractCommand(Guid.NewGuid(), 50);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        // Arrange
        var command = new UpdateContractCommand(Guid.Empty, 50);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Contract ID is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Should_HaveError_When_EquipmentQuantityIsNotPositive(int quantity)
    {
        // Arrange
        var command = new UpdateContractCommand(Guid.NewGuid(), quantity);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must be greater than zero");
    }

    [Fact]
    public void Should_HaveError_When_EquipmentQuantityExceedsMaximum()
    {
        // Arrange
        var command = new UpdateContractCommand(Guid.NewGuid(), 1001);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must not exceed 1000");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    public void Should_Pass_With_ValidQuantities(int quantity)
    {
        // Arrange
        var command = new UpdateContractCommand(Guid.NewGuid(), quantity);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EquipmentQuantity);
    }
}

public class DeactivateContractCommandValidatorTests
{
    private readonly DeactivateContractCommandValidator _validator;

    public DeactivateContractCommandValidatorTests()
    {
        _validator = new DeactivateContractCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_IdIsValid()
    {
        // Arrange
        var command = new DeactivateContractCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_IdIsEmpty()
    {
        // Arrange
        var command = new DeactivateContractCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Contract ID is required");
    }
}

public class CreateContractDtoValidatorTests
{
    private readonly CreateContractDtoValidator _validator;

    public CreateContractDtoValidatorTests()
    {
        _validator = new CreateContractDtoValidator();
    }

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var dto = new CreateContractDto(
            "FAC-001",
            "EQT-001",
            10
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_When_ProductionFacilityCodeIsEmpty(string facilityCode)
    {
        // Arrange
        var dto = new CreateContractDto(facilityCode, "EQT-001", 10);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode);
    }

    [Fact]
    public void Should_HaveError_When_ProductionFacilityCodeExceedsMaxLength()
    {
        // Arrange
        var longCode = new string('A', 51);
        var dto = new CreateContractDto(longCode, "EQT-001", 10);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode)
            .WithErrorMessage("Production facility code must not exceed 50 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_When_ProcessEquipmentTypeCodeIsEmpty(string equipmentCode)
    {
        // Arrange
        var dto = new CreateContractDto("FAC-001", equipmentCode, 10);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProcessEquipmentTypeCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_When_EquipmentQuantityIsNotPositive(int quantity)
    {
        // Arrange
        var dto = new CreateContractDto("FAC-001", "EQT-001", quantity);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must be greater than zero");
    }

    [Fact]
    public void Should_HaveError_When_EquipmentQuantityExceedsMaximum()
    {
        // Arrange
        var dto = new CreateContractDto("FAC-001", "EQT-001", 1001);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity)
            .WithErrorMessage("Equipment quantity must not exceed 1000");
    }

    [Fact]
    public void Should_HaveMultipleErrors_When_MultipleFieldsAreInvalid()
    {
        // Arrange
        var dto = new CreateContractDto("", "", 0);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductionFacilityCode);
        result.ShouldHaveValidationErrorFor(x => x.ProcessEquipmentTypeCode);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentQuantity);
        result.Errors.Should().HaveCountGreaterThan(2);
    }
}