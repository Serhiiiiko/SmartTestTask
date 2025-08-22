using MediatR;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Commands;

public record DeactivateContractCommand(Guid Id) : IRequest<Result>;