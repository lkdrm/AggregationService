using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AggregationService.Infrastructure.Handlers;

/// <summary>
/// Globally handles unhandled exceptions thrown during the request pipeline,
/// returning a structured <see cref="ProblemDetails"/> JSON response.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GlobalExceptionHandler"/>.
    /// </summary>
    /// <param name="logger">The logger used to record exception details.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Attempts to handle the given <paramref name="exception"/> by writing a
    /// <see cref="ProblemDetails"/> JSON response to the <paramref name="httpContext"/>.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The unhandled exception to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// <see langword="true"/> to indicate the exception was handled and
    /// no further handlers should be invoked.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, $"Error caught by global handler: {exception.Message}");

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = exception.Message,
            // Combines the HTTP method and path to identify the failing endpoint.
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            problemDetails.Extensions.Add("correlationId", correlationId?.ToString());
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}