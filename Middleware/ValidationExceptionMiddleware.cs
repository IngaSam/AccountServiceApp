using AccountService.Models.Errors;
using AccountService.Models.Results;
using FluentValidation;
using System.Text.Json;

namespace AccountService.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().ErrorMessage
                );

            var result = JsonSerializer.Serialize(MbResult<object>.Fail(
                new MbError("VALIDATION_ERROR", "Validation failed", errors)
            ));

            await context.Response.WriteAsync(result);
        }
    }
}