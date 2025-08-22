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
public class FacilitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FacilitiesController> _logger;

    public FacilitiesController(IMediator mediator, ILogger<FacilitiesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all production facilities
    /// </summary>
    /// <returns>List of facilities with area information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductionFacilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllFacilities()
    {
        var query = new GetAllFacilitiesQuery();
        var result = await _mediator.Send(query);
        
        return result.Match<IActionResult>(
            onSuccess: facilities => Ok(facilities),
            onFailure: error => Problem(
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Failed to retrieve facilities"));
    }
}