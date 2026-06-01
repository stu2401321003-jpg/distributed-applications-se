using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.API.Extensions;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.Users;
using RentACarAPI.Application.Users.Contracts;

namespace RentACarAPI.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [RequireUserId]
    [HttpGet("me")]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var me = await _userService.GetMeAsync(userId, cancellationToken);
        return Ok(me);
    }
}
