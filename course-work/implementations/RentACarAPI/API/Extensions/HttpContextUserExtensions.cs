using RentACarAPI.API.Filters;

namespace RentACarAPI.API.Extensions;

public static class HttpContextUserExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(RequireUserIdAttribute.HttpContextItemKey, out var value) && value is int userId)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Invalid token.");
    }
}
