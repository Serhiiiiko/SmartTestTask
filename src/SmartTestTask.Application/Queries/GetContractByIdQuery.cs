using MediatR;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Queries;

public record GetContractByIdQuery(Guid Id) : IRequest<Result<ContractDto>>;