using MediatR;
using SmartTestTask.Application.DTOs.Responce;

namespace SmartTestTask.Application.Commands;

public class UpdateContractCommand : IRequest<ApiResponse<ContractDto>>
{
    public Guid Id { get; set; }
    public int EquipmentQuantity { get; set; }
    
    public UpdateContractCommand(Guid id, int equipmentQuantity)
    {
        Id = id;
        EquipmentQuantity = equipmentQuantity;
    }
}