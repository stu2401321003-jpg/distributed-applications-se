using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.Locations;
using RentACarAPI.Application.Locations.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<LocationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _locationService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _locationService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LocationResponse>> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var created = await _locationService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
