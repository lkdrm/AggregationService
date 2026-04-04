using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AggregationService.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting request {typeof(TRequest).Name}");
        var timer = Stopwatch.StartNew();

        var response = await next(cancellationToken);

        timer.Stop();
        _logger.LogInformation($"Completed request {typeof(TRequest).Name}, in {timer.ElapsedMilliseconds}");

        return response;
    }
}
