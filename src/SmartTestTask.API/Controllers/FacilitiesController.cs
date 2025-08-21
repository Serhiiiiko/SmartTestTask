using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Application.Queries;

namespace SmartTestTask.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductionFacilityDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFacilities()
    {
        try
        {
            var query = new GetAllFacilitiesQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facilities");
            return StatusCode(500, ApiResponse<IEnumerable<ProductionFacilityDto>>.FailureResponse("An error occurred while retrieving facilities"));
        }
    }
}
