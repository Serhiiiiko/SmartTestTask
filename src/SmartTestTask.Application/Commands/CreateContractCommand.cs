using MediatR;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Commands;

public record CreateContractCommand(
    string ProductionFacilityCode,
    string ProcessEquipmentTypeCode,
    int EquipmentQuantity) : IRequest<Result<ContractDto>>;