using FluentValidation;
using SmartTestTask.Application.Commands;

namespace SmartTestTask.Application.Validators;

public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contract ID is required");

        RuleFor(x => x.EquipmentQuantity)
            .GreaterThan(0).WithMessage("Equipment quantity must be greater than zero")
            .LessThanOrEqualTo(1000).WithMessage("Equipment quantity must not exceed 1000");
    }
}