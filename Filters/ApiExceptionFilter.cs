using AccountService.Exceptions;
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
                // Обработка ошибок валидации FluentValidation
                ValidationException ex => new BadRequestObjectResult(
                    new
                    {
                        Error = "Validation error",
                        Details = ex.Errors.Select(e => new {
                            Property = e.PropertyName,
                            Message = e.ErrorMessage
                        })
                    }),

                // Обработка бизнес-ошибок
                AccountNotFoundException ex => new NotFoundObjectResult(
                    new { Error = ex.Message }),

                CurrencyNotSupportedException ex => new BadRequestObjectResult(
                    new { Error = ex.Message }),

                InsufficientFundsException ex => new BadRequestObjectResult(
                    new { Error = ex.Message }),

                // Обработка всех остальных исключений
                _ => new ObjectResult(new
                {
                    Error = "Internal server error",
                    Details = context.Exception.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
            context.ExceptionHandled = true;
        }
    }
    
}
