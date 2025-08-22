using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;

namespace SmartTestTask.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
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
    [ProducesResponseType(typeof(IEnumerable<ProcessEquipmentTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllEquipmentTypes()
    {
        var query = new GetAllEquipmentTypesQuery();
        var result = await _mediator.Send(query);
        
        return result.Match<IActionResult>(
            onSuccess: equipmentTypes => Ok(equipmentTypes),
            onFailure: error => Problem(
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Failed to retrieve equipment types"));
    }
}