using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.API.Extensions;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.Payments;
using RentACarAPI.Application.Payments.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var created = await _paymentService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PaymentResponse>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? method,
        [FromQuery] int? userId,
        [FromQuery] int? rentalId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsQuery(status, method, userId, rentalId, from, to);
        var items = await _paymentService.GetAllAsync(query, cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("by-rental/{rentalId:int}")]
    public async Task<ActionResult<PaymentResponse>> GetByRentalId(int rentalId, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetByRentalIdAsync(rentalId, cancellationToken);
        return Ok(payment);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("by-rental/{rentalId:int}/pay")]
    public async Task<ActionResult<PaymentResponse>> MarkPaid(int rentalId, CancellationToken cancellationToken)
    {
        var updated = await _paymentService.MarkPaidAsync(rentalId, cancellationToken);
        return Ok(updated);
    }

    [Authorize]
    [RequireUserId]
    [HttpPost("mine")]
    public async Task<ActionResult<PaymentResponse>> CreateMine([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var created = await _paymentService.CreateForUserAsync(request, userId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyCollection<PaymentResponse>>> Mine(
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var query = new GetMyPaymentsQuery(status, from, to);
        var items = await _paymentService.GetMineAsync(userId, query, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("mine/by-rental/{rentalId:int}")]
    public async Task<ActionResult<PaymentResponse>> GetMineByRentalId(int rentalId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var payment = await _paymentService.GetByRentalIdForUserAsync(rentalId, userId, cancellationToken);
        return Ok(payment);
    }

    [Authorize]
    [RequireUserId]
    [HttpPut("mine/by-rental/{rentalId:int}/pay")]
    public async Task<ActionResult<PaymentResponse>> PayMine(int rentalId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var updated = await _paymentService.MarkPaidForUserAsync(rentalId, userId, cancellationToken);
        return Ok(updated);
    }
}
