using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Application.CarCategories;
using RentACarAPI.Application.CarCategories.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/car-categories")]
public sealed class CarCategoriesController : ControllerBase
{
    private readonly ICarCategoryService _carCategoryService;

    public CarCategoriesController(ICarCategoryService carCategoryService)
    {
        _carCategoryService = carCategoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CarCategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _carCategoryService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarCategoryResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _carCategoryService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CarCategoryResponse>> Create([FromBody] CreateCarCategoryRequest request, CancellationToken cancellationToken)
    {
        var created = await _carCategoryService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
