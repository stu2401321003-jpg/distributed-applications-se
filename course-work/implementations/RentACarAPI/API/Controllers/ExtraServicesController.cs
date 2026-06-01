using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.ExtraServices;
using RentACarAPI.Application.ExtraServices.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/extra-services")]
public sealed class ExtraServicesController : ControllerBase
{
    private readonly IExtraServiceService _extraServiceService;

    public ExtraServicesController(IExtraServiceService extraServiceService)
    {
        _extraServiceService = extraServiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ExtraServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _extraServiceService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ExtraServiceResponse>> Create([FromBody] CreateExtraServiceRequest request, CancellationToken cancellationToken)
    {
        var created = await _extraServiceService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
