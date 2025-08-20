using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Application.Commands;
using SmartTestTask.Application.DTOs.Request;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;

namespace SmartTestTask.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContractListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllContracts()
    {
        try
        {
            var query = new GetAllContractsQuery();
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts");
            return StatusCode(500, ApiResponse<IEnumerable<ContractListDto>>.FailureResponse("An error occurred while retrieving contracts"));
        }
    }

    /// <summary>
    /// Get a specific contract by ID
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>Contract details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContractById(Guid id)
    {
        try
        {
            var query = new GetContractByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contract {ContractId}", id);
            return StatusCode(500, ApiResponse<ContractDto>.FailureResponse("An error occurred while retrieving the contract"));
        }
    }

    /// <summary>
    /// Get contracts by facility code
    /// </summary>
    /// <param name="facilityCode">Facility code</param>
    /// <returns>List of contracts for the facility</returns>
    [HttpGet("facility/{facilityCode}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContractDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContractsByFacility(string facilityCode)
    {
        try
        {
            var query = new GetContractsByFacilityQuery(facilityCode);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contracts for facility {FacilityCode}", facilityCode);
            return StatusCode(500, ApiResponse<IEnumerable<ContractDto>>.FailureResponse("An error occurred while retrieving contracts"));
        }
    }

    /// <summary>
    /// Create a new equipment placement contract
    /// </summary>
    /// <param name="dto">Contract creation data</param>
    /// <returns>Created contract</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ContractDto>.FailureResponse("Validation failed", errors));
            }

            var command = new CreateContractCommand(
                dto.ProductionFacilityCode,
                dto.ProcessEquipmentTypeCode,
                dto.EquipmentQuantity);
            
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetContractById), new { id = result.Data.Id }, result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return StatusCode(500, ApiResponse<ContractDto>.FailureResponse("An error occurred while creating the contract"));
        }
    }

    /// <summary>
    /// Update an existing contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <param name="dto">Update data</param>
    /// <returns>Updated contract</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateContract(Guid id, [FromBody] UpdateContractDto dto)
    {
        try
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<ContractDto>.FailureResponse("Contract ID mismatch"));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ContractDto>.FailureResponse("Validation failed", errors));
            }

            var command = new UpdateContractCommand(dto.Id, dto.EquipmentQuantity);
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            if (result.Message.Contains("not found"))
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract {ContractId}", id);
            return StatusCode(500, ApiResponse<ContractDto>.FailureResponse("An error occurred while updating the contract"));
        }
    }

    /// <summary>
    /// Deactivate a contract
    /// </summary>
    /// <param name="id">Contract ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivateContract(Guid id)
    {
        try
        {
            var command = new DeactivateContractCommand(id);
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            if (result.Message.Contains("not found"))
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating contract {ContractId}", id);
            return StatusCode(500, ApiResponse<bool>.FailureResponse("An error occurred while deactivating the contract"));
        }
    }
}