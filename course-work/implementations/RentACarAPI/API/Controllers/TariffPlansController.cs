using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.TariffPlans;
using RentACarAPI.Application.TariffPlans.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/tariff-plans")]
public sealed class TariffPlansController : ControllerBase
{
    private readonly ITariffPlanService _tariffPlanService;

    public TariffPlansController(ITariffPlanService tariffPlanService)
    {
        _tariffPlanService = tariffPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TariffPlanResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _tariffPlanService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TariffPlanResponse>> Create([FromBody] CreateTariffPlanRequest request, CancellationToken cancellationToken)
    {
        var created = await _tariffPlanService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{tariffPlanId:int}/cars/{carId:int}")]
    public async Task<IActionResult> AssignToCar(int tariffPlanId, int carId, CancellationToken cancellationToken)
    {
        await _tariffPlanService.AssignToCarAsync(carId, tariffPlanId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{tariffPlanId:int}/cars/{carId:int}")]
    public async Task<IActionResult> UnassignFromCar(int tariffPlanId, int carId, CancellationToken cancellationToken)
    {
        await _tariffPlanService.UnassignFromCarAsync(carId, tariffPlanId, cancellationToken);
        return NoContent();
    }
}
