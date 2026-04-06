using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AggregationService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs the start and completion of every request,
/// including elapsed time in milliseconds.
/// </summary>
/// <typeparam name="TRequest">The type of the MediatR request.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LoggingBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger used to record request lifecycle events.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Intercepts the request pipeline to log the request name before and after handler execution.
    /// </summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">The next delegate in the pipeline representing the actual handler.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The response produced by the inner handler.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request {RequestName}", typeof(TRequest).Name);
        var timer = Stopwatch.StartNew();

        var response = await next(cancellationToken);

        timer.Stop();
        _logger.LogInformation("Completed request {RequestName}, in {ElapsedMs}", typeof(TRequest).Name, timer.ElapsedMilliseconds);

        return response;
    }
}
