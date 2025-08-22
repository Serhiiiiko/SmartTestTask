using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Request;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;
using SmartTestTask.Domain.Results;

namespace SmartTestTask.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ContractsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContractsController> _logger;

    public ContractsController(IMediator mediator, ILogger<ContractsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all equipment placement contracts
    /// </summary>
    /// <returns>List of contracts with facility and equipment information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContractListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllContracts()
    {
        var query = new GetAllContractsQuery();
        var result = await _mediator.Send(query);
        
        return result.Match<IActionResult>(
            onSuccess: contracts => Ok(contracts),
            onFailure: error => Problem(
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Failed to retrieve contracts"));
    }

    /// <summary>
    /// Get a specific contract by ID
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>Contract details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContractById(Guid id)
    {
        var query = new GetContractByIdQuery(id);
        var result = await _mediator.Send(query);
        
        return result.Match<IActionResult>(
            onSuccess: contract => Ok(contract),
            onFailure: error => error.Code.Contains("NotFound") 
                ? NotFound(new ProblemDetails 
                { 
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound,
                    Title = "Contract Not Found"
                })
                : Problem(detail: error.Message));
    }

    /// <summary>
    /// Create a new equipment placement contract
    /// </summary>
    /// <param name="dto">Contract creation data</param>
    /// <returns>Created contract</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ContractDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateContractCommand(
            dto.ProductionFacilityCode,
            dto.ProcessEquipmentTypeCode,
            dto.EquipmentQuantity);
        
        var result = await _mediator.Send(command);
        
        return result.Match<IActionResult>(
            onSuccess: contract => CreatedAtAction(
                nameof(GetContractById), 
                new { id = contract.Id }, 
                contract),
            onFailure: error => error.Code switch
            {
                "Facility.InsufficientArea" => Conflict(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status409Conflict,
                    Title = "Insufficient Area"
                }),
                "Facility.NotFound" or "Equipment.NotFound" => NotFound(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource Not Found"
                }),
                _ => BadRequest(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Request"
                })
            });
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="dto">Update data</param>
    /// <returns>Updated contract</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateContract(Guid id, [FromBody] UpdateContractDto dto)
    {
        if (id != dto.Id)
        {
            ModelState.AddModelError("Id", "Contract ID mismatch");
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new UpdateContractCommand(dto.Id, dto.EquipmentQuantity);
        var result = await _mediator.Send(command);
        
        return result.Match<IActionResult>(
            onSuccess: contract => Ok(contract),
            onFailure: error => error.Code switch
            {
                "Contract.NotFound" => NotFound(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound,
                    Title = "Contract Not Found"
                }),
                "Facility.InsufficientArea" => Conflict(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status409Conflict,
                    Title = "Insufficient Area"
                }),
                _ => BadRequest(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Request"
                })
            });
    }

    /// <summary>
    /// Deactivate a contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivateContract(Guid id)
    {
        var command = new DeactivateContractCommand(id);
        var result = await _mediator.Send(command);
        
        return result.Match<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => error.Code.Contains("NotFound")
                ? NotFound(new ProblemDetails
                {
                    Detail = error.Message,
                    Status = StatusCodes.Status404NotFound,
                    Title = "Contract Not Found"
                })
                : Problem(detail: error.Message));
    }
}