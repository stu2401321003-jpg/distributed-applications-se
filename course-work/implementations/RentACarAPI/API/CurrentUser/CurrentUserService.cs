using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RentACarAPI.API.Filters;
using RentACarAPI.Application.CurrentUser;

namespace RentACarAPI.API.CurrentUser;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return null;
        }

        if (httpContext.Items.TryGetValue(RequireUserIdAttribute.HttpContextItemKey, out var value) && value is int cached)
        {
            return cached;
        }

        ClaimsPrincipal? user = httpContext.User;
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(sub, out var userId) ? userId : null;
    }

    public int GetUserIdOrThrow()
    {
        var userId = GetUserId();
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        return userId.Value;
    }
}
