using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AggregationService.Infrastructure.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

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