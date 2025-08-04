using AccountService.Exceptions;
using AccountService.Models.Errors;
using AccountService.Models.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AccountService.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {


            var result = context.Exception switch
            {
                // Обработка ошибок валидации FluentValidation
                ValidationException ex => HandleValidationException(ex),


                // Обработка бизнес-ошибок
                AccountNotFoundException ex => new NotFoundObjectResult(
                    MbResult<object>.Fail("ACCOUNT_NOT_FOUND", ex.Message)),

                CurrencyNotSupportedException ex => new BadRequestObjectResult(
                    MbResult<object>.Fail("UNSUPPORTED_CURRENCY", ex.Message)),

                InsufficientFundsException ex => new BadRequestObjectResult(
                    MbResult<object>.Fail("UNSUPPORTED_CURRENCY", ex.Message)),

                // Обработка всех остальных исключений
                _ => new ObjectResult(
                    MbResult<object>.Fail("INTERNAL_ERROR", "Произошла внутренняя ошибка сервера"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
            context.Result = result;
            context.ExceptionHandled = true;
        }

        private static IActionResult HandleValidationException(ValidationException ex)
        {
            // Берем первую ошибку для каждого поля
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .Select(g => g.First())
                .ToList();

            // Если есть кастомный MbError в State - используем его
            var firstError = errors.First();
            var mbError = firstError.CustomState as MbError
                          ?? MbError.Create("VALIDATION_ERROR", firstError.ErrorMessage);

            return new BadRequestObjectResult(MbResult<object>.Fail(mbError));
        }

    }

}
