using System.Text.Json;
using RentACarAPI.Application.Common;

namespace RentACarAPI.API.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (ApiConflictException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new { message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
