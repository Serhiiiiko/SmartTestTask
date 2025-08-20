using MediatR;
using SmartTestTask.Application.DTOs.Responce;

namespace SmartTestTask.Application.Commands;

public class DeactivateContractCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    
    public DeactivateContractCommand(Guid id)
    {
        Id = id;
    }
}