using FluentValidation;
using SmartTestTask.Application.Commands;

namespace SmartTestTask.Application.Validators;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.ProductionFacilityCode)
            .NotEmpty().WithMessage("Production facility code is required")
            .MaximumLength(50).WithMessage("Production facility code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Production facility code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.ProcessEquipmentTypeCode)
            .NotEmpty().WithMessage("Process equipment type code is required")
            .MaximumLength(50).WithMessage("Process equipment type code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Process equipment type code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.EquipmentQuantity)
            .GreaterThan(0).WithMessage("Equipment quantity must be greater than zero")
            .LessThanOrEqualTo(1000).WithMessage("Equipment quantity must not exceed 1000");
    }
}