using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RentACarAPI.Application.CurrentUser;

namespace RentACarAPI.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireUserIdAttribute : Attribute, IAsyncActionFilter
{
    public const string HttpContextItemKey = "UserId";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        try
        {
            var userId = currentUser.GetUserIdOrThrow();
            context.HttpContext.Items[HttpContextItemKey] = userId;
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Result = new UnauthorizedObjectResult(new { message = ex.Message });
            return;
        }

        await next();
    }
}
