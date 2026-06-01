using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.API.Extensions;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.Cars.Contracts;
using RentACarAPI.Application.Favorites;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/users/me/favorites")]
public sealed class FavoritesController : ControllerBase
{
    private readonly IFavoritesService _favoritesService;

    public FavoritesController(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;
    }

    [Authorize]
    [RequireUserId]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CarResponse>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var items = await _favoritesService.GetMineAsync(userId, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [RequireUserId]
    [HttpPost("{carId:int}")]
    public async Task<IActionResult> Add(int carId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        await _favoritesService.AddAsync(userId, carId, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [RequireUserId]
    [HttpDelete("{carId:int}")]
    public async Task<IActionResult> Remove(int carId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        await _favoritesService.RemoveAsync(userId, carId, cancellationToken);
        return NoContent();
    }
}
