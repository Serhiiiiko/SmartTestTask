using MediatR;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.Application.Queries;

public record GetContractsByFacilityQuery(string FacilityCode) : IRequest<Result<IEnumerable<ContractDto>>>;