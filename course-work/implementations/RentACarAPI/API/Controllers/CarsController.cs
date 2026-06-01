using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.Cars;
using RentACarAPI.Application.Cars.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/cars")]
public sealed class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CarResponse>>> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] int? locationId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var items = await _carService.GetAllAsync(categoryId, locationId, isActive, cancellationToken);
        return Ok(items);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyCollection<CarResponse>>> Available(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int pickupLocationId,
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        var items = await _carService.GetAvailableAsync(from, to, pickupLocationId, categoryId, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _carService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CarResponse>> Create([FromBody] CreateCarRequest request, CancellationToken cancellationToken)
    {
        var created = await _carService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/image")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<CarResponse>> UploadImage(int id, IFormFile image, CancellationToken cancellationToken)
    {
        var updated = await _carService.UploadImageAsync(id, image, Request, cancellationToken);
        return Ok(updated);
    }
}
