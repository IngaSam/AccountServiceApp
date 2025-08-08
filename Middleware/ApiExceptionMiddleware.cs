using AccountService.Models.Errors;
using AccountService.Models.Results;
using System.Text.Json;

namespace AccountService.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(MbResult<object>.Fail(
                new MbError("SERVER_ERROR", "Internal server error")
            ));

            await context.Response.WriteAsync(result);
        }
    }
}