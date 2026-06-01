using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.API.Extensions;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.Rentals;
using RentACarAPI.Application.Rentals.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/rentals")]
public sealed class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<ActionResult<RentalResponse>> Create([FromBody] CreateRentalRequest request, CancellationToken cancellationToken)
    {
        var created = await _rentalService.CreateFromReservationAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RentalResponse>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] int? userId,
        [FromQuery] int? reservationId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = new GetRentalsQuery(status, userId, reservationId, from, to);
        var items = await _rentalService.GetAllAsync(query, cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id:int}/close")]
    public async Task<ActionResult<RentalResponse>> Close(int id, [FromBody] CloseRentalRequest request, CancellationToken cancellationToken)
    {
        var updated = await _rentalService.CloseAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentalResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var rental = await _rentalService.GetByIdAsync(id, cancellationToken);
        return Ok(rental);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<RentalDetailsResponse>> GetDetails(int id, CancellationToken cancellationToken)
    {
        var details = await _rentalService.GetDetailsAsync(id, cancellationToken);
        return Ok(details);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost("{id:int}/extras")]
    public async Task<ActionResult<RentalDetailsResponse>> AddExtra(int id, [FromBody] AddRentalExtraServiceRequest request, CancellationToken cancellationToken)
    {
        var details = await _rentalService.AddExtraAsync(id, request, cancellationToken);
        return Ok(details);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpDelete("{id:int}/extras/{extraServiceId:int}")]
    public async Task<ActionResult<RentalDetailsResponse>> RemoveExtra(int id, int extraServiceId, CancellationToken cancellationToken)
    {
        var details = await _rentalService.RemoveExtraAsync(id, extraServiceId, cancellationToken);
        return Ok(details);
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyCollection<RentalResponse>>> Mine(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var items = await _rentalService.GetMineAsync(userId, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine/{id:int}")]
    public async Task<ActionResult<RentalResponse>> MineById(int id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var item = await _rentalService.GetMineByIdAsync(id, userId, cancellationToken);
        return Ok(item);
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine/{id:int}/details")]
    public async Task<ActionResult<RentalDetailsResponse>> MineDetails(int id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var details = await _rentalService.GetMyDetailsAsync(id, userId, cancellationToken);
        return Ok(details);
    }

    [Authorize]
    [RequireUserId]
    [HttpPost("mine/{id:int}/extras")]
    public async Task<ActionResult<RentalDetailsResponse>> AddExtraMine(int id, [FromBody] AddRentalExtraServiceRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var details = await _rentalService.AddExtraForUserAsync(id, userId, request, cancellationToken);
        return Ok(details);
    }

    [Authorize]
    [RequireUserId]
    [HttpDelete("mine/{id:int}/extras/{extraServiceId:int}")]
    public async Task<ActionResult<RentalDetailsResponse>> RemoveExtraMine(int id, int extraServiceId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var details = await _rentalService.RemoveExtraForUserAsync(id, userId, extraServiceId, cancellationToken);
        return Ok(details);
    }
}
