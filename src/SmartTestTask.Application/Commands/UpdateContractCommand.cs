using MediatR;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Commands;

public record UpdateContractCommand(
    Guid Id,
    int EquipmentQuantity) : IRequest<Result<ContractDto>>;