using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;

namespace SmartTestTask.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentTypesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EquipmentTypesController> _logger;

    public EquipmentTypesController(IMediator mediator, ILogger<EquipmentTypesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all process equipment types
    /// </summary>
    /// <returns>List of equipment types</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProcessEquipmentTypeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllEquipmentTypes()
    {
        try
        {
            var query = new GetAllEquipmentTypesQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving equipment types");
            return StatusCode(500, ApiResponse<IEnumerable<ProcessEquipmentTypeDto>>.FailureResponse("An error occurred while retrieving equipment types"));
        }
    }
}