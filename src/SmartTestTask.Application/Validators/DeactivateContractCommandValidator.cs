using FluentValidation;
using SmartTestTask.Application.Commands;

namespace SmartTestTask.Application.Validators;

public class DeactivateContractCommandValidator : AbstractValidator<DeactivateContractCommand>
{
    public DeactivateContractCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contract ID is required");
    }
}