using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.TariffPlans;
using RentACarAPI.Application.TariffPlans.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/cars/{carId:int}/tariff-plans")]
public sealed class CarTariffPlansController : ControllerBase
{
    private readonly ITariffPlanService _tariffPlanService;

    public CarTariffPlansController(ITariffPlanService tariffPlanService)
    {
        _tariffPlanService = tariffPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TariffPlanResponse>>> GetForCar(int carId, CancellationToken cancellationToken)
    {
        var items = await _tariffPlanService.GetForCarAsync(carId, cancellationToken);
        return Ok(items);
    }
}
