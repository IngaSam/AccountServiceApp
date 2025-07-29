using MediatR;

namespace AccountService.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            logger.LogInformation($"Handling {typeof(TRequest).Name}");
            var response = await next(ct);
            logger.LogInformation($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }
}
