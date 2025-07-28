using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AccountService.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = context.Exception switch
            {
                ValidationException ex => new BadRequestObjectResult(
                    new { Error = ex.Errors.Select(e => e.ErrorMessage) }),
                _ => new ObjectResult(new { error = context.Exception.Message })
                {
                    StatusCode = 500
                }
            };
            context.ExceptionHandled = true;
        }
    }
    
}
