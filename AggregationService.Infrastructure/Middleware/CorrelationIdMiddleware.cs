using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AggregationService.Infrastructure.Middleware;

/// <summary>
/// Middleware that ensures every HTTP request has a correlation ID for distributed tracing.
/// If the incoming request does not contain a correlation ID header, a new GUID is generated.
/// The correlation ID is propagated to the response headers and made available via <see cref="HttpContext.Items"/>.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>The HTTP header name used to carry the correlation ID.</summary>
    private const string CorrelationIdHeader = "X-Correlation-Id";

    /// <summary>
    /// Initializes a new instance of <see cref="CorrelationIdMiddleware"/>.
    /// </summary>
    /// <param name="logger">Logger used to scope log entries under the current correlation ID.</param>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Processes an HTTP request by resolving or generating a correlation ID,
    /// attaching it to the response, and invoking the next middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items["CorrelationId"] = correlationId.ToString();

        using (_logger.BeginScope($"CorrelationId: {correlationId}"))
        {
            await _next(context);
        }
    }
}