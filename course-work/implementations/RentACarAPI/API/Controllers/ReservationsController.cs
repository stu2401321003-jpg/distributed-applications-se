using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.API.Extensions;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.Reservations;
using RentACarAPI.Application.Reservations.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> Mine(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var items = await _reservationService.GetMineAsync(userId, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [RequireUserId]
    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var created = await _reservationService.CreateAsync(request, userId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize]
    [RequireUserId]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        await _reservationService.CancelAsync(id, userId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ReservationListItemResponse>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] int? userId,
        [FromQuery] int? carId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = new GetReservationsQuery(status, userId, carId, from, to);
        var items = await _reservationService.GetAllAsync(query, cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDetailsResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _reservationService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<ReservationResponse>> Approve(int id, CancellationToken cancellationToken)
    {
        var updated = await _reservationService.ApproveAsync(id, cancellationToken);
        return Ok(updated);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<ReservationResponse>> Reject(int id, CancellationToken cancellationToken)
    {
        var updated = await _reservationService.RejectAsync(id, cancellationToken);
        return Ok(updated);
    }
}
