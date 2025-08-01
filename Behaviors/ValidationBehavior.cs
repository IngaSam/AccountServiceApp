﻿using FluentValidation;
using MediatR;

namespace AccountService.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators):
        IPipelineBehavior<TRequest, TResponse>
    where TRequest: IRequest<TResponse>
    {
        //private readonly IEnumerable<IValidator<TRequest>> _validators;
        //public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            if (!validators.Any()) return await next(ct);

            var context = new ValidationContext<TRequest>(request);
            var failures = validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);

            return await next(ct);
        }

    }
}
